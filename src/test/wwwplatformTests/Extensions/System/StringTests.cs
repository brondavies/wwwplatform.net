using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatformTests.Support;

namespace wwwplatform.Shared.Extensions.System.Tests
{
    [TestClass]
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

        [TestMethod]
        public void CleanFileNameTest()
        {
            Assert.AreEqual("abcdefghijklmnopqrstuvwxyz1234567890",
                "abcdefghijklmnopqrstuvwxyz1234567890".CleanFileName());

            Assert.AreEqual("33af996c-5d1d-48e5-975c-5c93cbb96cd8",
                "33af996c-5d1d-48e5-975c-5c93cbb96cd8".CleanFileName());

            Assert.AreEqual("The-quick-brown-fox-jumps-over-the-lazy-dog",
                "The quick brown fox jumps over the lazy dog.".CleanFileName());

            Assert.AreEqual("x-3d-j5-929v-68b3",
                "`-x,3d]j5+929v;68b3".CleanFileName());

            Assert.AreEqual("AbcdeFghijkLmnopQRstuvwxyz1234567890",
                "AbcdeFghijkLmnopQRstuvwxyz1234567890".CleanFileName());

            Assert.AreEqual("a-bn-c-ds-g-hj-k-s-x-n-i-9-m-sd-df5",
                "a!bn@c#ds$g%hj^k&*s(x)n+i_9:'m<sd>.df5".CleanFileName());

            string nullString = null;
            Assert.IsNull(nullString.CleanFileName());
        }

        [TestMethod]
        public void IsLocalPathTest()
        {
            Assert.IsFalse("".IsRootedPath());

            Assert.IsFalse("test".IsRootedPath());

            Assert.IsFalse("/this/is/a/test".IsRootedPath());

            Assert.IsTrue(@"\\this\is\a\test".IsRootedPath());

            Assert.IsTrue(@"C:\Windows\System32".IsRootedPath());

            Assert.IsTrue(@"\Windows\System32".IsRootedPath());

            string nullString = null;
            Assert.IsFalse(nullString.IsRootedPath());
        }
    }
}