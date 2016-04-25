using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace wwwplatform.Shared.Helpers
{
    public class DocumentHelper
    {
        private static string[] valid_document_extensions
        {
            get
            {
                return new string[] {
                ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".ppt", ".pptx", ".rtf", ".txt", ".wps", ".xps", ".epub", ".ppsx", ".ods", ".csv"
                };
            }
        }

        public static bool IsDocumentFile(string extension)
        {
            return valid_document_extensions.Contains(extension.ToLower());
        }

        public static bool IsDocumentFile(HttpPostedFileBase file)
        {
            return IsDocumentFile(Path.GetExtension(file.FileName));
        }
    }
}
