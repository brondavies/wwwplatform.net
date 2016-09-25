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
using System.Web.Routing;
using wwwplatform.Models.Serializers;
using Newtonsoft.Json;
using System.Web.Caching;
using Newtonsoft.Json.Serialization;
using wwwplatform.Extensions.Helpers;

namespace wwwplatform.Extensions
{
    public class BaseController : Controller
    {
        private ApplicationDbContext _db;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

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
            _db.CurrentUser = HttpContext.User;
            return _db;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        protected IdentityRole PublicRole
        {
            get
            {
                return RoleManager.FindByName(Roles.Public);
            }
        }

        private wwwplatform.Models.Settings _settings;
        public wwwplatform.Models.Settings Settings
        {
            get
            {
                if (_settings == null) { _settings = Settings.Create(HttpContext); }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }
        
        private int? _UserTimeZoneOffset;
        public int UserTimeZoneOffset
        {
            get
            {
                if (_UserTimeZoneOffset.HasValue)
                {
                    return _UserTimeZoneOffset.Value;
                }
                var cookie = HttpContext.Request.Cookies.Get("_tz");
                if (cookie != null)
                {
                    int result;
                    if (int.TryParse(cookie.Value, out result))
                    {
                        _UserTimeZoneOffset = result;
                        return result;
                    }
                }
                return 0;
            }
        }

        protected virtual JsonResult OK()
        {
            return Json(new { status = 200 }, JsonRequestBehavior.AllowGet);
        }

        protected virtual ActionResult Auto(object model, string viewName = null)
        {
            if (ClientAcceptsJson())
            {
                return Content(Serialize(model), "application/json");
            }
            if (string.IsNullOrEmpty(viewName))
            {
                return View(model);
            }
            return View(viewName, model);
        }

        private bool ClientAcceptsJson()
        {
            bool accept = (HttpContext.Request.AcceptTypes != null) && (
                HttpContext.Request.AcceptTypes.Contains("application/json") ||
                HttpContext.Request.AcceptTypes.Contains("text/javascript"));
            accept = accept || (HttpContext.Request.ContentType?.EndsWith("json")).GetValueOrDefault();
            return accept;
        }

        private string Serialize(object model)
        {
            var serializer = GetExecutingControllerSerializer();
            if (serializer == null)
            {
                return JsonConvert.SerializeObject(model);
            }
            JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = serializer };
            return JsonConvert.SerializeObject(model, settings);
        }

        private DefaultContractResolver GetExecutingControllerSerializer()
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(ControllerContext.Controller.GetType());
            var action = RouteData.GetRequiredString("action");
            var actionDescriptor = controllerDescriptor.FindAction(ControllerContext, action);
            Attributes.SerializerAttribute attribute = (Attributes.SerializerAttribute)
                (actionDescriptor.GetCustomAttributes(typeof(Attributes.SerializerAttribute), true).FirstOrDefault() ??
                 controllerDescriptor.GetCustomAttributes(typeof(Attributes.SerializerAttribute), true).FirstOrDefault());
            if (attribute != null)
            {
                return CacheHelper.GetFromCacheOrDefault<DefaultContractResolver>(HttpContext, attribute.Serializer);
            }
            return null;
        }

        protected virtual ActionResult ErrorResult(ModelStateDictionary modelState)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var errors = GetErrorsFromModelState(modelState);
            return Json(new { status = Response.StatusCode, errors = errors }, JsonRequestBehavior.AllowGet);
        }

        protected Dictionary<string, string[]> GetErrorsFromModelState(ModelStateDictionary modelState)
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

        private const string failureCookie = "_failure";
        private const string successCookie = "_success";

        protected void SetSuccessMessage(string message)
        {
            Response.SetCookie(new HttpCookie(successCookie, message));
        }

        protected void SetSuccessMessage(string message, params object[] args)
        {
            Response.SetCookie(new HttpCookie(successCookie, string.Format(message, args)));
        }

        protected void SetFailureMessage(string message)
        {
            Response.SetCookie(new HttpCookie(failureCookie, message));
        }

        protected void SetFailureMessage(string message, params object[] args)
        {
            Response.SetCookie(new HttpCookie(failureCookie, string.Format(message, args)));
        }

        protected new ActionResult HttpNotFound()
        {
            throw new HttpException(404, "Not found");
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            if (!ModelState.IsValid && !Response.HeadersWritten)
            {
                var errors = ErrorsFromModelState(ModelState);
                SetFailureMessage(errors);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
