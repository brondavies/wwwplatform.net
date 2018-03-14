using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Extensions.Helpers;
using wwwplatformTests.Support;

namespace wwwplatformTests.Extensions.Helpers
{
    [TestClass]
    public class BootstrapElementTests : TestBase
    {
        [TestMethod]
        public void TestFormatBytes()
        {
            Assert.AreEqual("1 B", FileStorage.FormatBytes(1));
            Assert.AreEqual("999 B", FileStorage.FormatBytes(999));
            Assert.AreEqual("1.1 KB", FileStorage.FormatBytes(1100));
            Assert.AreEqual("983 KB", FileStorage.FormatBytes(1006592));
            Assert.AreEqual("1.1 MB", FileStorage.FormatBytes(1118435));
            Assert.AreEqual("999 MB", FileStorage.FormatBytes(1047500888));
            Assert.AreEqual("1.1 GB", FileStorage.FormatBytes(1180008880));
            Assert.AreEqual("110 GB", FileStorage.FormatBytes(118110000000));
            Assert.AreEqual("2.6 TB", FileStorage.FormatBytes(2857100000000));
        }
    }
}