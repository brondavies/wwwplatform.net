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
using wwwplatform.Extensions.Email;
using wwwplatform.Extensions.Logging;
using wwwplatform.Models.ViewModels;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize(Roles.Administrators, Roles.ListManagers)]
    public class MailingListsController : BaseController
    {
        private const string AllowedFields = "Id,Name,Description,EmailAddress";

        // GET: MailingLists
        public async Task<ActionResult> Index()
        {
            return View(await db.ActiveMailingLists.ToListAsync());
        }

        // GET: MailingLists/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }
            return View(mailingList);
        }

        // GET: MailingLists/Create
        public ActionResult Create()
        {
            return View();
        }

        // GET: MailingLists/Compose
        public async Task<ActionResult> Compose(long id)
        {
            var mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }
            return View(mailingList);
        }

        // POST: MailingLists/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Send(long id, ComposeEmailMessageModel model)
        {
            var mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }
            var subscribers = mailingList.Subscribers.Where(s => s.Enabled).ToList();
            var SentCount = 0;
            var TotalCount = 0;
            bool success = true;
            try
            {
                EmailSender sender = new EmailSender(model.subject, model.body, "");
                sender.Settings = Settings;
                sender.IsHTML = true;
                sender.Body = model.body;
                sender.FromEmail = mailingList.EmailAddress;
                sender.Subject = model.subject;
                if (!string.IsNullOrEmpty(model.attachments))
                {
                    sender.Attachments = new List<string>(model.attachments.Split(';'));
                }
                
                TotalCount = subscribers.Count();
                foreach (var address in subscribers)
                {
                    sender.ToName = (address.FirstName + " " + address.LastName).Trim();
                    sender.Addresslist = address.Email;
#if DEBUG
                    sender.Execute(true);
#else
                    sender.Execute(false);
#endif
                }
                SentCount = sender.Sent;
                
            }
            catch (Exception e)
            {
                Log.Error(e);
                SetFailureMessage(e.Message);
                success = false;
            }

            return View(new SentEmailMessageModel { SentCount = SentCount, TotalCount = TotalCount, Subject = model.subject, To = mailingList.Name, Success = success });
        }

        // POST: MailingLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = AllowedFields)] MailingList mailingList)
        {
            if (ModelState.IsValid)
            {
                db.MailingLists.Add(mailingList);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(mailingList);
        }

        // GET: MailingLists/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }
            return View(mailingList);
        }

        // POST: MailingLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = AllowedFields)] MailingList mailingList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mailingList).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(mailingList);
        }

        // GET: MailingLists/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }
            return View(mailingList);
        }

        // POST: MailingLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
            db.MailingLists.Remove(mailingList);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Subscribe(long? id)
        {
            MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }

            return View(mailingList);
        }

        [HttpPost, ActionName("Subscribe")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubscriberCreate(long id, [Bind(Include = "FirstName,LastName,Email")] MailingListSubscriber mailingListSubscriber)
        {
            if (ModelState.IsValid)
            {
                MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
                if (mailingList == null)
                {
                    return HttpNotFound();
                }
                mailingListSubscriber.MailingList = mailingList;
                db.MailingListSubscribers.Add(mailingListSubscriber);
                //TODO: send confirmation email
                await db.SaveChangesAsync();
            }

            return View(mailingListSubscriber);
        }

        [HttpGet]
        public async Task<ActionResult> AddSubscriber(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MailingList mailingList = await db.ActiveMailingLists.FindAsync(id);
            if (mailingList == null)
            {
                return HttpNotFound();
            }

            return View(new MailingListSubscriber { MailingList = mailingList });
        }
    }
}
