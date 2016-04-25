using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Moq;
using System;
using System.Net;
using System.Web.Mvc;
using wwwplatform.Models;
using wwwplatformTests.Support;

namespace wwwplatform.Controllers.Tests
{
    public class ControllerTestBase : TestBase
    {
        internal string controllerName;

        override public void Initialize()
        {
            base.Initialize();
        }

        protected ApplicationSignInManager MockApplicationSignInManager()
        {
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockAuthenticationManager = new Mock<AuthenticationManager>();
            var mock = new Mock<ApplicationSignInManager>(MockApplicationUserManager(), mockAuthenticationManager.Object);
            mock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(SignInStatus.Success);
            return mock.Object;
        }

        protected ApplicationUserManager MockApplicationUserManager()
        {
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<ApplicationUserManager>(mockStore.Object);
            mock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            return mock.Object;
        }

        protected MockControllerContext CreateMockContext(Controller controller, string action)
        {
            var context = new MockControllerContext(controller, controllerName, action);
            context.Request.Setup(r => r.Url).Returns(new Uri("https://localhost/" + controllerName + "/" + action));
            return context;
        }
    }
}