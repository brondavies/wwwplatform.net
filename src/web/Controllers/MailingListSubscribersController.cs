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
using wwwplatform.Extensions.Email;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators, Roles.ListManagers)]
    public class MailingListSubscribersController : BaseController
    {
        private const string AllowedFields = "Id,FirstName,LastName,Email,Enabled";

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Create(long id)
        {
            var mailinglist = await db.ActiveMailingLists.FindAsync(id);
            if (mailinglist == null)
            {
                return HttpNotFound();
            }
            return View(new MailingListSubscriber { MailingList = mailinglist });
        }

        // POST: MailingListSubscribers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Create(long id, [Bind(Include = AllowedFields)] MailingListSubscriber mailingListSubscriber)
        {
            if (ModelState.IsValid)
            {
                var mailinglist = await db.ActiveMailingLists.FindAsync(id);
                if (mailinglist != null)
                {
                    MailingListSubscriber existing = await db.ActiveMailingListSubscribers.FirstOrDefaultAsync(s => s.Email == mailingListSubscriber.Email && s.MailingList.Id == mailinglist.Id);
                    if (existing == null)
                    {
                        mailinglist.Subscribers.Add(mailingListSubscriber);
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        mailingListSubscriber = existing;
                    }
                    if (!mailingListSubscriber.Enabled)
                    {
                        EmailSender sender = new EmailSender(
                            mailinglist.Name + " - Confirm Subscription", 
                            string.Format("Confirm your email address by following this link: <br/><a href=\"{0}\">{0}</a>", 
                                string.Format("http://{0}/MailingListSubscribers/Verify/{1}", Settings.CanonicalHostName, mailingListSubscriber.Verification)),
                            mailingListSubscriber.Email);
                        sender.ToName = mailingListSubscriber.FullName();
                        sender.IsHTML = true;
                        sender.Execute();
                    }

                    if (Roles.UserInAnyRole(User, Roles.ListManagers))
                    {
                        return RedirectToMailingList(id);
                    }
                    else
                    {
                        return RedirectToAction("Confirm");
                    }
                }
                else
                {
                    return HttpNotFound();
                }
            }

            return View(mailingListSubscriber);
        }

        // GET: MailingListSubscribers/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers.FindAsync(id);
            if (mailingListSubscriber == null)
            {
                return HttpNotFound();
            }

            return View(mailingListSubscriber);
        }

        // POST: MailingListSubscribers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] MailingListSubscriber subscriber)
        {
            if (ModelState.IsValid)
            {
                MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers.FindAsync(subscriber.Id);
                if (mailingListSubscriber == null)
                {
                    return HttpNotFound();
                }
                mailingListSubscriber.Update(subscriber);

                db.Entry(mailingListSubscriber).State = EntityState.Modified;
                await db.SaveChangesAsync();

                SetSuccessMessage("List subscriber {0} was updated successfully!", mailingListSubscriber.FullName());

                return RedirectToMailingList(mailingListSubscriber.MailingList.Id);
            }

            return Auto(subscriber);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Confirm()
        {
            return View();
        }

        [HttpGet]
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

            return RedirectToAction("Verified");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Verified()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Unsubscribe(string id)
        {
            MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers.Where(m => m.Verification == id).FirstOrDefaultAsync();

            return View(mailingListSubscriber ?? new MailingListSubscriber());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Unsubscribe(long id, string email)
        {
            MailingListSubscriber mailingListSubscriber = await db.ActiveMailingListSubscribers.FindAsync(id);
            if (mailingListSubscriber != null)
            {
                db.MailingListSubscribers.Remove(mailingListSubscriber);
                await db.SaveChangesAsync();
            }
            if (!string.IsNullOrEmpty(email))
            {
                var subs = db.ActiveMailingListSubscribers.Where(m => m.Email == email).ToList();
                if (subs != null && subs.Count > 0)
                {
                    db.MailingListSubscribers.RemoveRange(subs);
                    await db.SaveChangesAsync();
                }
            }

            return RedirectToAction("Unsubscribed");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Unsubscribed()
        {
            return View();
        }

        // GET: MailingListSubscribers/Delete/5
        [HttpGet]
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var mailingListSubscriber = await db.ActiveMailingListSubscribers.FindAsync(id);
            if (mailingListSubscriber == null)
            {
                return HttpNotFound();
            }
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
            var mailingListId = mailingListSubscriber.MailingList.Id;
            db.MailingListSubscribers.Remove(mailingListSubscriber);
            await db.SaveChangesAsync();

            SetSuccessMessage("{0} was removed successfully!", mailingListSubscriber.FullName());

            return RedirectToMailingList(mailingListId);
        }

        private RedirectToRouteResult RedirectToMailingList(long mailingListId)
        {
            return RedirectToAction("Details", "MailingLists", new { id = mailingListId });
        }
    }
}
