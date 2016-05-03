using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Extensions;

namespace wwwplatform.Extensions.Tests
{
    [TestClass()]
    public class StringExtensionsTests
    {
        [TestMethod()]
        public void RandomTest()
        {
            Assert.AreEqual(32, String.Random().Length);

            Assert.AreEqual(16, String.Random(length: 16).Length);

            Assert.AreNotEqual(String.Random(), String.Random());

            Assert.AreEqual(String.Random(seed: 1), String.Random(seed: 1));

            Assert.AreNotEqual(String.Random(seed: 2), String.Random(seed: 3));
        }
    }
}