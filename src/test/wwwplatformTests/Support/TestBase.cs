using Moq;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Models;

namespace wwwplatformTests.Support
{
    public class TestBase
    {
        public virtual void Initialize()
        {
        }

        private ApplicationDbContext _db;
        protected ApplicationDbContext db
        {
            get
            {
                if (_db == null)
                {
                    _db = ApplicationDbContext.Create();
                }
                return _db;
            }
        }

        #region Mock Objects

        protected HttpContextBase MockHttpContextWrapper()
        {
            var wrapper = new Mock<HttpContextBase>();
            wrapper.Setup(x => x.Request).Returns(MockRequest());
            wrapper.Setup(x => x.Response).Returns(MockResponse());
            wrapper.Setup(x => x.Server).Returns(MockServer());

            return wrapper.Object;
        }

        protected HttpResponseBase MockResponse()
        {
            var Response = new Mock<HttpResponseBase>();
            Response.Setup(x => x.ApplyAppPathModifier(It.IsAny<string>())).Returns("/app/path");
            return Response.Object;
        }

        protected HttpRequestBase MockRequest()
        {
            var Request = new Mock<HttpRequestBase>();
            Uri uri;
            Uri.TryCreate("http://localhost/", UriKind.Absolute, out uri);
            Request.Setup(x => x.Url).Returns(uri);
            return Request.Object;
        }

        protected HttpServerUtilityBase MockServer()
        {
            var Server = new Mock<HttpServerUtilityBase>();
            Server.Setup(x => x.MapPath(It.IsAny<string>())).Returns(@"C:\inetpub\wwwplatform.net");
            return Server.Object;
        }

        protected ViewContext MockViewContext(TextWriter textWriter)
        {
            var controller = new MvcTestController();
            var mockContext = new MockControllerContext(controller, "Test", "Index");
            var controllerContext = controller.ControllerContext;
            var view = new Mock<IView>().Object;
            var viewData = new ViewDataDictionary();
            var tempData = new TempDataDictionary();

            return new System.Web.Mvc.ViewContext(controllerContext, view, viewData, tempData, textWriter);
        }

        protected string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        static Random randomizer = new Random();
        static char[] chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        protected string RandomString(int length = 16)
        {
            string result = "";
            while (result.Length < length)
            {
                result += chars[randomizer.Next(0, chars.Length)];
            }
            return result;
        }

        #endregion
    }
}
