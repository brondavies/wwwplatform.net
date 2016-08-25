using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using wwwplatform.Extensions.Helpers.Bootstrap;
using wwwplatformTests.Support;
using System.Text;
using System.IO;

namespace wwwplatformTests.Extensions.Helpers.Bootstrap
{
    [TestClass]
    public class BootstrapElementTests : TestBase
    {
        [TestMethod]
        public void TestEmptyElement()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                using (BootstrapElement element = new BootstrapElement(MockViewContext(textWriter)))
                {
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual("<div class=\"\"></div>", stringBuilder.ToString());
        }

        [TestMethod]
        public void TestElementWithText()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                using (BootstrapElement element = new BootstrapElement(MockViewContext(textWriter)))
                {
                    element.TagName = "h1";
                    element.ClassName = "text-center";
                    element.Text = "This is a test";
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual("<h1 class=\"text-center\">This is a test</h1>", stringBuilder.ToString());
        }

        [TestMethod]
        public void TestElementWithChild()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                var context = MockViewContext(textWriter);
                using (BootstrapElement element = new BootstrapElement(context))
                {
                    element.ClassName = "outer";
                    element.Text = "I am a parent";
                    element.Add(new BootstrapElement(context) { ClassName = "inner", Text = "I am a child" });
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual("<div class=\"outer\">I am a parent<div class=\"inner\">I am a child</div></div>", stringBuilder.ToString());
        }

        [TestMethod]
        public void TestElementWithChildren()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                var context = MockViewContext(textWriter);
                using (BootstrapElement element = new BootstrapElement(context))
                {
                    element.ClassName = "outer";
                    element.Text = "I am a parent";
                    element.Add(new BootstrapElement(context) { ClassName = "inner", Text = "I am a child" });
                    element.Add(new BootstrapElement(context) { ClassName = "inner2", Text = "I am another child" });
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual(
                "<div class=\"outer\">" +
                    "I am a parent" +
                    "<div class=\"inner\">I am a child</div>" +
                    "<div class=\"inner2\">I am another child</div>" +
                "</div>",
                stringBuilder.ToString());
        }

        [TestMethod]
        public void TestElementsWithParent()
        {
            var stringBuilder = new StringBuilder();
            using (var textWriter = new StringWriter(stringBuilder))
            {
                var context = MockViewContext(textWriter);
                using (BootstrapElement element = new BootstrapElement(context))
                {
                    element.ClassName = "outer";
                    element.Text = "I am a parent";
                    using (var child1 = new BootstrapElement(context) { ClassName = "inner", Text = "I am a child", Parent = element })
                    {
                        child1.ToMvcHtmlString();
                    }
                    using (var child2 = new BootstrapElement(context) { ClassName = "inner2", Text = "I am another child", Parent = element })
                    {
                        child2.ToMvcHtmlString();
                    }
                    element.ToMvcHtmlString();
                }
            }

            Assert.AreEqual(
                "<div class=\"outer\">" +
                    "I am a parent" +
                    "<div class=\"inner\">I am a child</div>" +
                    "<div class=\"inner2\">I am another child</div>" +
                "</div>",
                stringBuilder.ToString());
        }
    }
}
