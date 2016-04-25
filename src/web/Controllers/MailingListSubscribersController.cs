using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Models;
using wwwplatform.Extensions;
using System.Web.Routing;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators, Roles.ListManagers)]
    public class MailingListSubscribersController : BaseController
    {
        // POST: MailingListSubscribers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Create(long id, [Bind(Include = "FirstName,LastName,Email,Enabled")] MailingListSubscriber mailingListSubscriber)
        {
            if (ModelState.IsValid)
            {
                var mailinglist = await db.ActiveMailingLists.FindAsync(id);
                if (mailinglist != null)
                {
                    MailingListSubscriber existing = await db.ActiveMailingListSubscribers.FirstOrDefaultAsync(s => s.Email == mailingListSubscriber.Email &&  s.MailingList.Id == mailinglist.Id );
                    if (existing == null)
                    {
                        mailinglist.Subscribers.Add(mailingListSubscriber);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Details", "MailingLists", new { id = mailingListSubscriber.MailingList.Id });
                    }
                    else
                    {
                        //return new ModelValidationResult { MemberName = "Email", Message = "Email is already subscribed." };
                        return RedirectToAction("Details", "MailingLists", new { id = mailingListSubscriber.MailingList.Id });
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return ErrorResult(ModelState);
        }

        // POST: MailingListSubscribers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(long id, bool Enabled)
        {
            MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers.FindAsync(id);
            if (mailingListSubscriber == null)
            {
                return HttpNotFound();
            }

            mailingListSubscriber.Enabled = Enabled;
            db.Entry(mailingListSubscriber).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return OK();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Verify(string id)
        {
            MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers
                .Where(m => m.Verification == id)
                .FirstOrDefaultAsync();
            if (mailingListSubscriber == null)
            {
                return HttpNotFound();
            }

            mailingListSubscriber.Enabled = true;
            db.Entry(mailingListSubscriber).State = EntityState.Modified;
            await db.SaveChangesAsync();

            return View(mailingListSubscriber);
        }

        // POST: MailingListSubscribers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id)
        {
            MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers.FindAsync(id);
            if (mailingListSubscriber == null)
            {
                return HttpNotFound();
            }
            db.MailingListSubscribers.Remove(mailingListSubscriber);
            await db.SaveChangesAsync();

            return OK();
        }
    }
}
