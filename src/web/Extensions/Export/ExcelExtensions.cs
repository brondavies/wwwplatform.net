using OfficeOpenXml;
using System.Collections.Generic;
using wwwplatform.Models;

namespace wwwplatform.Extensions.Export
{
    public static class ExcelExtensions
    {
        public static void Export(this IEnumerable<MailingListSubscriber> subscribers, ExcelWorksheet worksheet)
        {
            worksheet.Row(1).Style.Fill.BackgroundColor.SetColor(0xFF, 0x3F, 0x51, 0xB5);
            worksheet.Row(1).Style.Font.Color.SetColor(0xFF, 0xFF, 0xFF, 0xFF);
            var col = 1;
            var row = 1;
            worksheet.Cells[row, col++].Value = "FirstName";
            worksheet.Cells[row, col++].Value = "LastName";
            worksheet.Cells[row, col++].Value = "Email";
            worksheet.Cells[row, col++].Value = "Verified";
            foreach (var subscriber in subscribers)
            {
                col = 1;
                row++;
                worksheet.Cells[row, col++].Value = subscriber.FirstName;
                worksheet.Cells[row, col++].Value = subscriber.LastName;
                worksheet.Cells[row, col++].Value = subscriber.Email;
                worksheet.Cells[row, col++].Value = subscriber.Enabled ? "Yes" : "No";
            }
            worksheet.Cells[1, 1, row, col].AutoFitColumns();
        }
    }
}