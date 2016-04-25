using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wwwplatform.Models;
using System.Web.Mvc;

namespace wwwplatform.Controllers.Tests
{
    [TestClass()]
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
        
        [TestMethod()]
        public void LoginTest()
        {
            AccountController controller = new AccountController();
            var context = CreateMockContext(controller, "Login");
            controller.UserManager = MockApplicationUserManager();
            var returnUrl = "/test";

            var result = controller.Login(returnUrl: returnUrl);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = (ViewResult)result;
            Assert.IsNotNull(viewResult.ViewBag);
            Assert.AreEqual(returnUrl, viewResult.ViewBag.ReturnUrl);
        }

        /* TODO: Unimplemented tests
                [TestMethod()]
                public void LoginTest1()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void VerifyCodeTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void VerifyCodeTest1()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void RegisterTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void RegisterTest1()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ConfirmEmailTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ForgotPasswordTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ForgotPasswordTest1()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ForgotPasswordConfirmationTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ResetPasswordTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ResetPasswordTest1()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ResetPasswordConfirmationTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ExternalLoginTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void SendCodeTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void SendCodeTest1()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ExternalLoginCallbackTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ExternalLoginConfirmationTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void LogOffTest()
                {
                    Assert.Fail();
                }

                [TestMethod()]
                public void ExternalLoginFailureTest()
                {
                    Assert.Fail();
                }
        */
    }
}