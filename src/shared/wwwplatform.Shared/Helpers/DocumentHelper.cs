using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using FTTLib;
using System.Diagnostics;

namespace wwwplatform.Shared.Helpers
{
    public class DocumentHelper
    {
        public static bool IsDocumentFile(string filename)
        {
            return FTT.GetFileCategory(filename) == FileCategory.Document;
        }

        public static bool IsDocumentFile(HttpPostedFileBase file)
        {
            return IsDocumentFile(file.FileName);
        }

        public static bool CreatePDF(string convertExe, string inputFile, string outputFile = null, bool wait = false)
        {
            if (outputFile == null)
            {
                outputFile = Path.ChangeExtension(inputFile, "pdf");
            }
            Process proc = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo(convertExe);
            startinfo.CreateNoWindow = true;
            startinfo.Arguments = string.Format("\"{0}\" \"{1}\"", inputFile, outputFile);
            proc.StartInfo = startinfo;
            bool result = proc.Start();
            if (result && wait)
            {
                proc.WaitForExit();
                result = proc.ExitCode == 0;
            }
            return result;
        }
    }
}
