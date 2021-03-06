﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using wwwplatform.Extensions.Logging;
using wwwplatform.Models;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Extensions.Email
{
    internal class Parameter
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    internal class EmailSender : IDisposable
    {
        public string Subject;
        public string FromEmail = null;
        public string FromName = null;
        public string Addresslist;
        public string ToName = null;
        public string BCC = null;
        public string CC = null;
        public int JobID = DateTime.Now.TimeOfDay.Seconds;
        public int ID = 0;
        public bool IsHTML = true;
        public string Body = "";

        public List<string> Attachments = null;

        public List<Parameter> AttachmentStreams = null;

        public int CommunicationType = -1;
        public int ChannelID = 0;

        public int Sent = 0;

        public string Error = null;

        private HttpContextBase _HttpContext;
        public HttpContextBase HttpContext
        {
            get
            {
                if (_HttpContext == null)
                {
                    _HttpContext = new HttpContextWrapper(System.Web.HttpContext.Current);
                }
                return _HttpContext;
            }
            set
            {
                _HttpContext = value;
            }
        }

        private Settings _Settings;
        public Settings Settings
        {
            get
            {
                if (_Settings == null)
                {
                    _Settings = Settings.Create(new HttpContextWrapper(System.Web.HttpContext.Current));
                }
                return _Settings;
            }
            set
            {
                _Settings = value;
            }
        }

        public EmailSender(string subject, string body, string addresses)
        {
            Subject = subject;
            Body = body;
            Addresslist = addresses;
        }

        SmtpClient SmtpMail;
        int batch = 0;
        public void Execute(bool testmode = false)
        {
            if (SmtpMail == null)
            {
                SmtpMail = new SmtpClient();
            }
            MailMessage objMM = new MailMessage();
            objMM.From = new MailAddress(FromEmail ?? Settings.EmailDefaultFrom, FromName ?? Settings.SiteName ?? Settings.EmailDefaultFrom);
            objMM.Subject = Subject;
            objMM.IsBodyHtml = IsHTML;
            objMM.Body = IsHTML ? PrepareBodyHtml(Body) : Body;
            objMM.Headers.Add("X-JOB", JobID.ToString());
            if (!string.IsNullOrEmpty(Settings.EmailAdditionalHeaders))
            {
                string[] additionalHeaders = Settings.EmailAdditionalHeaders.Split(';');
                foreach(var header in additionalHeaders)
                {
                    string[] parts = header.Split('=');
                    objMM.Headers.Add(parts[0], parts[1]);
                    //objMM.Headers.Add("X-MC-InlineCSS", "true");
                }
            }

            if (Attachments != null)
            {
                foreach (string att in Attachments)
                {
                    objMM.Attachments.Add(new Attachment(att));
                }
            }

            if (AttachmentStreams != null)
            {
                foreach (Parameter ps in AttachmentStreams)
                {
                    objMM.Attachments.Add(new Attachment((System.IO.Stream)ps.Value, ps.Name));
                }
            }

            string[] addresses = Addresslist.Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in addresses)
            {
                try
                {
                    objMM.To.Add(new MailAddress(address, ToName));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            
            addresses = (CC ?? "").Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in addresses)
            {
                try
                {
                    objMM.CC.Add(new MailAddress(address));
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            addresses = (BCC ?? "").Split(";, \r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in addresses)
            {
                try
                {
                    objMM.Bcc.Add(new MailAddress(address));
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    Log.Error(e);
                }
            }

            try
            {
                batch++;
                if (testmode)
                {
                    SmtpMail.EnableSsl = false;
                    SmtpMail.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    SmtpMail.PickupDirectoryLocation = System.IO.Directory.CreateDirectory(
                        (@"~/App_Data/smtpout/" + GetFirstTo(objMM.To)).ResolveLocalPath()).FullName;
                }
                SmtpMail.Send(objMM);
                Sent++;
            }
            catch (Exception e)
            {
                if (testmode)
                {
                    throw e;
                }
                else
                {
                    Log.Error(e);
                }
            }

            if (batch >= Settings.MaxEmailSendBatch)
            {
                batch = 0;
                SmtpMail = null;
            }
        }

        private string PrepareBodyHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var links = doc.DocumentNode.Descendants("a");
            foreach (var link in links)
            {
                string href = link.GetAttributeValue("href", "");
                if (!string.IsNullOrEmpty(href))
                {
                    if (!href.StartsWith("http"))
                    {
                        href = href.ToAbsoluteUrl(HttpContext, HttpContext.Request.IsSecureConnection);
                        link.Attributes["href"].Value = href;
                    }
                }
            }
            return doc.DocumentNode.OuterHtml;
        }

        private string GetFirstTo(MailAddressCollection to)
        {
            string first = "none";
            if (to.Count() > 0)
            {
                var email = to.First();
                if (email != null)
                {
                    first = email.Address;
                }
            }
            return first;
        }

        bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (SmtpMail != null)
                {
                    SmtpMail.Dispose();
                    SmtpMail = null;
                }
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}