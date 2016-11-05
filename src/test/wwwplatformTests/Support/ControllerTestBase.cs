using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Moq;
using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Models;
using wwwplatformTests.Support;
using System.Web.Routing;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;

namespace wwwplatform.Controllers.Tests
{
    public class ControllerTestBase : TestBase
    {
        internal string controllerName;

        override public void Initialize()
        {
            base.Initialize();
            db.Setup();
            ApplicationDbContext.Upgrade();
        }

        #region Mock Objects

        protected ApplicationSignInManager MockApplicationSignInManager(SignInStatus returns = SignInStatus.Success)
        {
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mockAuthenticationManager = new Mock<IAuthenticationManager>();
            var mock = new Mock<ApplicationSignInManager>(MockApplicationUserManager(), mockAuthenticationManager.Object);
            mock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(returns);
            return mock.Object;
        }

        protected ApplicationUserManager MockApplicationUserManager(Action<Mock<ApplicationUserManager>> setup = null)
        {
            var mockStore = new Mock<IUserStore<ApplicationUser>>();
            var mock = new Mock<ApplicationUserManager>(mockStore.Object);
            mock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            setup?.Invoke(mock);
            return mock.Object;
        }

        protected MockControllerContext CreateMockContext(Controller controller, string action)
        {
            var context = new MockControllerContext(controller, controllerName, action);
            context.Request.Setup(r => r.Url).Returns(new Uri("https://localhost/" + controllerName + "/" + action));
            controller.Url = controller.Url ?? new UrlHelper(MockRequestContext());
            return context;
        }

        protected RequestContext MockRequestContext()
        {
            var requestContext = new RequestContext(MockHttpContextWrapper(), new RouteData());
            return requestContext;
        }

        protected IPrincipal CreateMockUser(string username, string usersRole = null)
        {
            var user = new Mock<IPrincipal>();
            //var mockIdentity = new Mock<IIdentity>();
            var mockIdClaim = new Claim("id", Guid.Empty.ToString());
            var mockIdentity = new Mock<ClaimsIdentity>();
            mockIdentity.Setup(i => i.FindFirst(It.IsAny<string>())).Returns(mockIdClaim);
            mockIdentity.Setup(i => i.IsAuthenticated).Returns(true);
            mockIdentity.Setup(i => i.Name).Returns(username);
            if (usersRole != null)
            {
                user.Setup(u => u.IsInRole(It.Is<string>(s => s == usersRole))).Returns(true);
            }
            //can't mock extension method mockIdentity.Setup(i => i.GetUserId()).Returns(Guid.Empty.ToString());
            user.Setup(u => u.Identity).Returns(mockIdentity.Object);
            return user.Object;
        }

        protected IPrincipal MockAnonymousUser()
        {
            var user = new Mock<IPrincipal>();
            var mockIdentity = new Mock<IIdentity>();
            mockIdentity.Setup(i => i.Name).Returns("Anonymous");
            mockIdentity.Setup(i => i.IsAuthenticated).Returns(false);
            user.Setup(u => u.Identity).Returns(mockIdentity.Object);
            return user.Object;
        }

        protected ApplicationRoleManager CreateRoleManager()
        {
            var rolemanager = new ApplicationRoleManager(new RoleStore<IdentityRole>(db));
            Roles.CreateAll(rolemanager);
            return rolemanager;
        }

        #endregion

    }
}