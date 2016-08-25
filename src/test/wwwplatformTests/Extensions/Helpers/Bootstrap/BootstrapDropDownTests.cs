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
                    element.FieldValue = "MyDropdown";
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual(
                "<div class=\"dropdown\">" +
                    "<button aria-haspopup=\"true\" class=\"btn dropdown-toggle\" data-toggle=\"dropdown\">" +
                    "MyDropdown <span class=\"caret\"></span>" +
                    "</button>" +
                    "<ul class=\"dropdown-menu\"></ul>" +
                "</div>",
                stringBuilder.ToString());
        }

        [TestMethod]
        public void TestDropDownWithOptions()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                using (BootstrapDropDown element = new BootstrapDropDown(MockViewContext(textWriter)))
                {
                    element.FieldName = "SomeField";
                    element.FieldValue = "Value1";
                    element.Options = new [] { new MenuOption { Value = "Value1" }, new MenuOption { Value = "Value2" } };
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual(
                "<div class=\"dropdown\">" +
                    "<input name=\"SomeField\" type=\"hidden\" value=\"Value1\" />" +
                    "<button aria-haspopup=\"true\" class=\"btn dropdown-toggle\" data-toggle=\"dropdown\">" +
                        "Value1 <span class=\"caret\"></span>" +
                    "</button>" +
                    "<ul class=\"dropdown-menu\">" +
                        "<li><a data-value=\"Value1\" href=\"#\">Value1</a></li>" +
                        "<li><a data-value=\"Value2\" href=\"#\">Value2</a></li>" +
                    "</ul>" +
                "</div>",
                stringBuilder.ToString());
        }
    }
}
