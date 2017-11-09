using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Controllers.Tests;
using wwwplatform.Controllers;
using wwwplatform.Models;
using System.Web.Mvc;
using System.Collections.Generic;
using Moq;
using wwwplatform;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;

namespace wwwplatformTests.Controllers
{
    [TestClass]
    public class SitePagesControllerTests : ControllerTestBase
    {
        [ClassInitialize]
        public static void FeatureTestClassInitialize(TestContext context)
        {
            wwwplatformTests.Support.Settings.Initialize(context);
        }

        [TestInitialize]
        public override void Initialize()
        {
            controllerName = "SitePages";
            base.Initialize();
        }

        [TestMethod]
        public void CreateTest()
        {
            var controller = new SitePagesController();
            var context = CreateMockContext(controller, "Create");
            controller.db = db;
            controller.RoleManager = CreateRoleManager();
            var mockUser = CreateMockUser("jethro");
            db.CurrentUser = mockUser;
            context.HttpContext.Setup(r => r.User).Returns(mockUser);

            var sitePage = new SitePage { Name = "Test site page", Description = "This is a test" };
            var usersRole = controller.RoleManager.FindByNameAsync(Roles.Users);
            usersRole.Wait();
            var usersRole_Id = usersRole.Result.Id;
            string[] permissions = new string[] { usersRole_Id };
            var result = controller.Create(sitePage, permissions);

            Assert.IsInstanceOfType(result.Result, typeof(RedirectToRouteResult));

            var viewResult = (RedirectToRouteResult)result.Result;

            Assert.AreEqual("Index", viewResult.RouteValues["action"]);
        }

        [TestMethod]
        public void IndexTest()
        {
            var controller = new SitePagesController();
            var context = CreateMockContext(controller, "Index");
            controller.db = db;
            controller.RoleManager = CreateRoleManager();
            controller.UserManager = MockApplicationUserManager((Mock<ApplicationUserManager> mockusermanager) =>
            {
                mockusermanager.Setup(m => m.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(new List<string>(new[] { Roles.Users }));
            });
            var owner = CreateMockUser("jethro");
            db.CurrentUser = owner;
            var usersRole = controller.RoleManager.FindByNameAsync(Roles.Users);
            usersRole.Wait();
            var usersRole_Id = usersRole.Result.Id;

            var sitePage = db.SitePages.Add(new SitePage
            {
                Name = RandomString(),
                Description = RandomString(),
                Slug = RandomString(),
                Permissions = new Permission[] { new Permission { AppliesToRole_Id = usersRole_Id } }
            });
            db.SaveChanges();

            var mockUser = CreateMockUser("tull", Roles.Users);

            db.CurrentUser = mockUser;
            context.HttpContext.Setup(r => r.User).Returns(mockUser);

            var result = controller.Index();

            Assert.IsInstanceOfType(result.Result, typeof(ViewResult));

            var viewResult = (ViewResult)result.Result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(List<SitePage>));

            var model = (List<SitePage>)viewResult.Model;

            Assert.IsNotNull(model.Find(s => s.Name == sitePage.Name));

            var anonymousUser = MockAnonymousUser();
            db.CurrentUser = anonymousUser;
            context.HttpContext.Setup(r => r.User).Returns(anonymousUser);
            controller.UserManager = MockApplicationUserManager();

            result = controller.Index();
            Assert.IsInstanceOfType(result.Result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult)result.Result).Model, typeof(List<SitePage>));
        }

        [TestMethod]
        public void DisplayTest()
        {
            var controller = new SitePagesController();
            var context = CreateMockContext(controller, "Display");
            controller.db = db;
            controller.RoleManager = CreateRoleManager();
            controller.UserManager = MockApplicationUserManager((Mock<ApplicationUserManager> mockusermanager) =>
            {
                mockusermanager.Setup(m => m.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(new List<string>(new[] { Roles.Users }));
            });
            var owner = CreateMockUser("jethro");
            db.CurrentUser = owner;
            var usersRole = controller.RoleManager.FindByNameAsync(Roles.Users);
            usersRole.Wait();
            var usersRole_Id = usersRole.Result.Id;

            var sitePage = db.SitePages.Add(new SitePage
            {
                Name = RandomString(),
                Description = RandomString(),
                Slug = RandomString(),
                Permissions = new Permission[] { new Permission { AppliesToRole_Id = usersRole_Id } }
            });
            db.SaveChanges();

            var mockUser = CreateMockUser("tull");

            db.CurrentUser = mockUser;
            context.HttpContext.Setup(r => r.User).Returns(mockUser);

            var result = controller.Display(sitePage.Slug);

            Assert.IsInstanceOfType(result.Result, typeof(ViewResult));

            var viewResult = (ViewResult)result.Result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(SitePage));

            var model = (SitePage)viewResult.Model;

            Assert.IsNotNull(model);
            Assert.AreEqual(sitePage.Name, model.Name);
        }

        [TestMethod]
        public void DisplayNotAllowedTest()
        {
            var controller = new SitePagesController();
            var context = CreateMockContext(controller, "Display");
            controller.db = db;
            controller.RoleManager = CreateRoleManager();
            controller.UserManager = MockApplicationUserManager((Mock<ApplicationUserManager> mockusermanager) =>
            {
                mockusermanager.Setup(m => m.GetRolesAsync(It.IsAny<string>())).ReturnsAsync(new List<string>(new[] { Roles.Users }));
            });
            var owner = CreateMockUser("jethro");
            db.CurrentUser = owner;
            var adminsRole = controller.RoleManager.FindByNameAsync(Roles.Administrators);
            adminsRole.Wait();
            var adminsRole_Id = adminsRole.Result.Id;

            var sitePage = db.SitePages.Add(new SitePage
            {
                Name = RandomString(),
                Description = RandomString(),
                Slug = RandomString(),
                Permissions = new Permission[] { new Permission { AppliesToRole_Id = adminsRole_Id } }
            });
            db.SaveChanges();

            var mockUser = CreateMockUser("tull");

            db.CurrentUser = mockUser;
            context.HttpContext.Setup(r => r.User).Returns(mockUser);

            var exceptionThrown = false;
            try
            {
                var result = controller.Display(sitePage.Slug);
                result.Wait(); // should throw exception
            }
            catch (HttpException httpex)
            {
                Assert.AreEqual(404, httpex.GetHttpCode());
                exceptionThrown = true;
            }
            catch (AggregateException agex)
            {
                Assert.IsInstanceOfType(agex.InnerException, typeof(HttpException));
                var inner = (HttpException)agex.InnerException;
                Assert.AreEqual(404, inner.GetHttpCode());
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}
