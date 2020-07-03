using OfficeOpenXml;
using System.Web.Mvc;

namespace wwwplatform.Extensions.Actions
{
    public class EPPlusResult : ActionResult
    {
        private ExcelPackage package;
        private readonly string filename;

        public EPPlusResult(ExcelPackage package, string filename = "download.xlsx")
        {
            this.package = package;
            this.filename = filename;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            context.HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
            package.SaveAs(context.HttpContext.Response.OutputStream);
        }
    }
}