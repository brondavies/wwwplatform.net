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
            startinfo.UseShellExecute = false;
            startinfo.Arguments = string.Format("\"{0}\" \"{1}\" /remote", inputFile, outputFile);
            proc.StartInfo = startinfo;
            bool result = proc.Start();
            if (result && wait)
            {
                proc.WaitForExit();
                result = proc.ExitCode == 0;
            }
            return result;
        }

        public static bool CreateThumbnail(string convertExe, string inputFile, string outputFile, int width, bool wait = false)
        {
            if (outputFile == null)
            {
                outputFile = Path.ChangeExtension(inputFile, "jpg");
            }
            Process proc = new Process();
            ProcessStartInfo startinfo = new ProcessStartInfo(convertExe);
            startinfo.CreateNoWindow = true;
            startinfo.UseShellExecute = false;
            startinfo.Arguments = string.Format("\"{0}\" \"{1}\" /w {2} /remote", inputFile, outputFile, width);
            proc.StartInfo = startinfo;
            bool result = proc.Start();
            if (result && wait)
            {
                proc.WaitForExit();
                result = proc.ExitCode == 0;
            }
            return result;
        }

        public static int CheckConverterSetup(string convertExe, string logFilename)
        {
            var process = new Process();
            int exitcode = -1;
            process.StartInfo = new ProcessStartInfo
            {
                FileName = convertExe,
                Arguments = "/test /silent /remote /log " + logFilename,
                CreateNoWindow = true,
                UseShellExecute = false,
                ErrorDialog = false
            };
            if (process.Start())
            {
                process.WaitForExit();
                exitcode = process.ExitCode;
            }
            return exitcode;
        }
    }
}
