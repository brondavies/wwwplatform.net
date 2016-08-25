using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Extensions.Helpers.Bootstrap;
using wwwplatformTests.Support;
using System.Text;
using System.IO;

namespace wwwplatformTests.Extensions.Helpers.Bootstrap
{
    [TestClass]
    public class BootstrapDropDownTests : TestBase
    {
        [TestMethod]
        public void TestBasicDropDown()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                using (BootstrapDropDown element = new BootstrapDropDown(MockViewContext(textWriter)))
                {
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual(
                "<div class=\"dropdown\">" +
                    "<button aria-haspopup=\"true\" class=\"btn dropdown-toggle\" data-toggle=\"dropdown\">" +
                    "<span class=\"caret\"></span>" +
                    "</button>" +
                    "<ul class=\"dropdown-menu\"></ul>" +
                "</div>",
                stringBuilder.ToString());
        }
    }
}
