using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wwwplatform.Models;
using System.Web;
using Moq;
using System.Web.Mvc;
using wwwplatform.Models.ViewModels;
using System.IO;
using System.Security.Principal;

namespace wwwplatform.Controllers.Tests
{
    [TestClass()]
    public class WebFilesControllerTests : ControllerTestBase
    {
        [ClassInitialize]
        public static void FeatureTestClassInitialize(TestContext context)
        {
            wwwplatformTests.Support.Settings.Initialize(context);
        }

        [TestInitialize]
        public override void Initialize()
        {
            controllerName = "WebFiles";
            base.Initialize();
        }

        [TestMethod]
        public void CreateTest()
        {
            var controller = new WebFilesController();
            var context = CreateMockContext(controller, "Create");
            controller.db = db;
            controller.RoleManager = CreateRoleManager();
            var mockUser = CreateMockUser("jethro");
            db.CurrentUser = mockUser;
            context.HttpContext.Setup(r => r.User).Returns(mockUser); //(new Mock<IPrincipal>().Object);
            var mockFile = new Mock<HttpPostedFileBase>();
            mockFile.Setup(f => f.ContentLength).Returns(100);
            mockFile.Setup(f => f.FileName).Returns("test_web_file.png");
            mockFile.Setup(f => f.SaveAs(It.IsAny<string>())).Callback((string filename) => { File.WriteAllText(filename, "test web file"); });
            var mockFiles = new Mock<HttpFileCollectionBase>();
            mockFiles.Setup(f => f.Count).Returns(1);
            mockFiles.Setup(f => f[It.IsAny<int>()]).Returns(mockFile.Object);
            context.Request.Setup(x => x.Files).Returns(() => mockFiles.Object);

            var webfile = new WebFile { Name = "Test web file" };
            var result = controller.Create(webfile);

            Assert.IsInstanceOfType(result.Result, typeof(ViewResult));

            var viewResult = (ViewResult)result.Result;

            Assert.IsInstanceOfType(viewResult.Model, typeof(UploadResults));

            var model = (UploadResults)viewResult.Model;

            Assert.AreEqual(model.status, UploadResults.OK);
        }
        /* TODO: finish unimplemented tests
        [TestMethod]
        public void IndexTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void DetailsTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void CreateTest1()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void EditTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void EditTest1()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void DeleteTest()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void DeleteConfirmedTest()
        {
            Assert.Fail();
        }
        */
    }
}