using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using wwwplatform.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace wwwplatform.Extensions
{
    public class BaseController : Controller
    {
        private ApplicationDbContext _db;

        public ApplicationDbContext db
        {
            get
            {
                return _db ?? GetDbContext();
            }
            set
            {
                _db = value;
            }
        }

        private ApplicationDbContext GetDbContext()
        {
            _db = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            return _db;
        }

        protected virtual JsonResult OK()
        {
            return Json(new { status = 200 }, JsonRequestBehavior.AllowGet);
        }

        protected virtual ActionResult ErrorResult(ModelStateDictionary modelState)
        {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            var errors = GetErrorsFromModelState(modelState);
            return Json(new { status = Response.StatusCode, errors = errors }, JsonRequestBehavior.AllowGet);
        }

        private Dictionary<string, string[]> GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string[]>();
            foreach (var item in modelState)
            {
                if (item.Value.Errors != null && item.Value.Errors.Count > 0)
                {
                    errors[item.Key] = item.Value.Errors.Select(e => e.ErrorMessage).ToArray();
                }
            }
            return errors;
        }

        protected string ErrorsFromModelState(ModelStateDictionary modelState)
        {
            List<string> list = new List<string>();
            foreach (string key in modelState.Keys)
            {
                var item = modelState[key];
                foreach (var error in item.Errors)
                {
                    list.Add(error.ErrorMessage);
                }
            }
            return list.Count > 0 ? string.Join("\r\n", list.ToArray()) : null;
        }

        protected string ErrorsFromException(Exception exception)
        {
            List<string> list = new List<string>();
            while (exception != null && !string.IsNullOrEmpty(exception.Message))
            {
                list.Add(exception.Message);
                exception = exception.InnerException;
            }
            return list.Count > 0 ? string.Join("\r\n", list.ToArray()) : null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
