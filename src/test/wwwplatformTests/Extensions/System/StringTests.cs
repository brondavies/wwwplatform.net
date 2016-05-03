using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatformTests.Support;

namespace wwwplatform.Shared.Extensions.System.Tests
{
    [TestClass()]
    public class StringTests : TestBase
    {
        [TestMethod]
        public void ToAppPathTest()
        {
            var context = MockHttpContextWrapper();
            string filename = @"C:\inetpub\wwwplatform.net\Content\myfile.png";
            Assert.AreEqual("/Content/myfile.png", filename.ToAppPath(context));
        }

        [TestMethod]
        public void ResolveUrlTest()
        {
            var context = MockHttpContextWrapper();
            Assert.AreEqual("/app/path", "~/My/File".ResolveUrl(context));
        }

        [TestMethod]
        public void ToAbsoluteUrlTest()
        {
            var context = MockHttpContextWrapper();
            Assert.AreEqual("http://localhost/app/path", "~/My/File".ToAbsoluteUrl(context));

            Assert.AreEqual("https://localhost/app/path", "~/My/File".ToAbsoluteUrl(context, true));

            string nullString = null;
            Assert.IsNull(nullString.ToAbsoluteUrl());
        }

        [TestMethod]
        public void MakeHttpTest()
        {
            Assert.AreEqual("http://www.example.com", "www.example.com".MakeHttp());
            Assert.AreEqual("https://www.example.com", "www.example.com".MakeHttp(true));

            string nullString = null;
            Assert.IsNull(nullString.MakeHttp());
        }

        [TestMethod]
        public void ResolveLocalPathTest()
        {
            var context = MockHttpContextWrapper();
            Assert.AreEqual(@"C:\inetpub\wwwplatform.net", "~/My/File".ResolveLocalPath(context));
        }

        [TestMethod]
        public void FromBase64StringTest()
        {
            Assert.AreEqual("abcdefghijklmnopqrstuvwxyz1234567890", "YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY3ODkw".FromBase64String());

            string nullString = null;
            Assert.IsNull(nullString.FromBase64String());
        }

        [TestMethod]
        public void ToBase64StringTest()
        {
            Assert.AreEqual("YWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY3ODkw",
                "abcdefghijklmnopqrstuvwxyz1234567890".ToBase64String());

            string nullString = null;
            Assert.IsNull(nullString.ToBase64String());
        }
    }
}