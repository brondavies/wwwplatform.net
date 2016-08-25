using Microsoft.AspNet.Identity.Owin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Models;
using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Moq;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using wwwplatformTests.Support;

namespace wwwplatform.Controllers.Tests
{
    [TestClass]
    public class AccountControllerTests : ControllerTestBase
    {

        [ClassInitialize]
        public static void FeatureTestClassInitialize(TestContext context)
        {
            wwwplatformTests.Support.Settings.Initialize(context);
        }

        [TestInitialize]
        public override void Initialize()
        {
            controllerName = "Account";
            base.Initialize();
        }
        
        [TestMethod]
        public void LoginTest()
        {
            var controller = new AccountController();
            var context = CreateMockContext(controller, "Login");
            controller.UserManager = MockApplicationUserManager();
            var returnUrl = "/test";

            var result = controller.Login(returnUrl: returnUrl);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.ViewBag);
            Assert.AreEqual(returnUrl, viewResult.ViewBag.ReturnUrl);
        }

        [TestMethod]
        public void Login_With_Username_Password_Success_Test()
        {
            var controller = new AccountController();
            var context = CreateMockContext(controller, "Login");
            controller.SignInManager = MockApplicationSignInManager();
            var returnUrl = "/test";

            LoginViewModel model = new LoginViewModel {
                Username = "user1",
                Password = "password1",
                RememberMe = true
            };
            var asyncresult = controller.Login(returnUrl: returnUrl, model:model);
            
            var result = asyncresult.Result;

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            var redirResult = (RedirectResult)result;
            Assert.AreEqual(returnUrl, redirResult.Url);
        }

        [TestMethod]
        public void Login_With_Username_Password_Failure_Test()
        {
            var controller = new AccountController();
            var context = CreateMockContext(controller, "Login");
            controller.SignInManager = MockApplicationSignInManager(returns: SignInStatus.Failure);
            var returnUrl = "/test";

            LoginViewModel model = new LoginViewModel
            {
                Username = "user1",
                Password = "password1",
                RememberMe = true
            };
            var asyncresult = controller.Login(returnUrl: returnUrl, model: model);

            var result = asyncresult.Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual("", viewResult.ViewName);
            Assert.IsInstanceOfType(viewResult.Model, typeof(LoginViewModel));
        }

        [TestMethod]
        public void RegisterTest()
        {
            var controller = new AccountController();
            controller.Settings = new Models.Settings();
            var context = CreateMockContext(controller, "Register");
            controller.RoleManager = CreateRoleManager();
            controller.UserManager = MockApplicationUserManager();
            controller.SignInManager = MockApplicationSignInManager();
            Roles.CreateAll(controller.RoleManager);
            
            var model = new RegisterViewModel
            {
                UserName = "user1",
                Password = "password1",
                ConfirmPassword = "password1",
                Email = "user1@example.com",
                FirstName = "User",
                LastName = "Test"
            };
            var asyncresult = controller.Register(model);

            var result = asyncresult.Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirResult = (RedirectToRouteResult)result;
            Assert.AreEqual("Home", redirResult.RouteValues["controller"]);
            Assert.AreEqual("Index", redirResult.RouteValues["action"]);

        }

        [TestMethod]
        public void ConfirmEmailTest()
        {
            var controller = new AccountController();
            var context = CreateMockContext(controller, "ConfirmEmail");
            controller.RoleManager = CreateRoleManager();
            controller.UserManager = MockApplicationUserManager((mock) => {
                mock.Setup(x => x.ConfirmEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            });
            controller.SignInManager = MockApplicationSignInManager();

            var userId = NewGuid();
            var code = Extensions.String.Random(4);
            
            var asyncresult = controller.ConfirmEmail(userId, code);

            var result = asyncresult.Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.AreEqual("ConfirmEmail", viewResult.ViewName);
        }

        [TestMethod]
        public void ForgotPasswordTest()
        {
            var controller = new AccountController();
            controller.Settings = new Models.Settings();
            var mockUrlHelper = new MockUrlHelper(MockRequestContext());
            mockUrlHelper.ActionCallback = (a, c, r, p) => {
                return string.Format("{0}://{1}/{2}/{3}/{4}", p, c, a, r["userId"], r["code"]);
            };
            controller.Url = mockUrlHelper;
            var context = CreateMockContext(controller, "ForgotPassword");
            var user = new ApplicationUser {
                Id = NewGuid(),
                Email = "user1@example.com"
            };
            var token = Extensions.String.Random();
            string resultUserId = null;
            string resultSubject = null;
            string resultBody = null;

            controller.SignInManager = MockApplicationSignInManager();
            controller.RoleManager = CreateRoleManager();
            controller.UserManager = MockApplicationUserManager((mock) => {
                mock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
                mock.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<string>())).ReturnsAsync(true);
                mock.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<string>())).ReturnsAsync(token);
                mock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult(user))
                    .Callback<string, string, string>((userId, subject, body) => {
                        resultUserId = userId;
                        resultSubject = subject;
                        resultBody = body;
                    });
            });

            var model = new ForgotPasswordViewModel {
                Email = user.Email
            };

            var asyncresult = controller.ForgotPassword(model);

            var result = asyncresult.Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirResult = (RedirectToRouteResult)result;
            Assert.AreEqual("ForgotPasswordConfirmation", redirResult.RouteValues["action"]);
            Assert.AreEqual(user.Id, resultUserId);
            Assert.AreEqual("Reset Password", resultSubject);
            Assert.IsTrue(resultBody.Contains(token));
            Assert.IsTrue(resultBody.Contains(user.Id));
        }

        [TestMethod]
        public void ForgotPasswordConfirmationTest()
        {
            var controller = new AccountController();
            controller.Settings = new Models.Settings();
            var context = CreateMockContext(controller, "ForgotPasswordConfirmation");

            var result = controller.ForgotPasswordConfirmation();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void ResetPasswordTest()
        {
            var controller = new AccountController();
            controller.Settings = new Models.Settings();
            var context = CreateMockContext(controller, "ResetPassword");
            var user = new ApplicationUser
            {
                Id = NewGuid(),
                Email = "user1@example.com"
            };
            controller.UserManager = MockApplicationUserManager((mock) => {
                mock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
                mock.Setup(x => x.ResetPasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);
            });

            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = user.Email,
                Code = Extensions.String.Random(),
                Password = "abcdefgh!",
                ConfirmPassword = "abcdefgh!",
            };
            var asyncResult = controller.ResetPassword(model: model);

            var result = asyncResult.Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            var redirResult = (RedirectToRouteResult)result;
            Assert.AreEqual("ResetPasswordConfirmation", redirResult.RouteValues["action"]);
            Assert.IsTrue(controller.ModelState.IsValid);
        }

        [TestMethod]
        public void ResetPasswordConfirmationTest()
        {
            var controller = new AccountController();
            controller.Settings = new Models.Settings();
            var context = CreateMockContext(controller, "ResetPasswordConfirmation");

            var result = controller.ResetPasswordConfirmation();

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        /* TODO: Unimplemented tests

            [TestMethod]
            public void VerifyCodeTest()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void ExternalLoginTest()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void SendCodeTest()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void ExternalLoginCallbackTest()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void ExternalLoginConfirmationTest()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void LogOffTest()
            {
                Assert.Fail();
            }

            [TestMethod]
            public void ExternalLoginFailureTest()
            {
                Assert.Fail();
            }
        */
    }
}