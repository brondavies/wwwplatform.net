using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ConvertPdf
{
    partial class ConvertPdf
    {
        #region Options

        private static bool checkSupport;
        private static bool installService;
        private static bool overwrite;
        private static bool uninstallService;
        private static bool useService;
        private static bool runService;
        private static bool silent;

        private static FileStream logfile;

        #endregion

        #region Constants

        private static string[] supportedFileTypes = new string[] {
            ".doc", ".docx", ".dot", ".dotx", ".pdf", ".ppt", ".pptm", ".pptx", ".xls", ".xlsb", ".xlsx"
        };

        const int WORD_INSTALLED = 1;
        const int PPT_INSTALLED = 2;
        const int EXCEL_INSTALLED = 4;
        const int GS_INSTALLED = 8;

        #endregion

        static void Main(string[] args)
        {
            ConvertPdfOptions Options = new ConvertPdfOptions();
            ParseArguments(args, Options);
            if (checkSupport)
            {
                Environment.ExitCode = CheckRequirements();
                return;
            }
            else if (installService)
            {
                Install();
                return;
            }
            else if (runService)
            {
                StartService();
                return;
            }
            else if (uninstallService)
            {
                Uninstall();
                return;
            }

            ExecuteConversion(Options);
        }

        private static void ExecuteConversion(ConvertPdfOptions Options)
        {
            if (ValidateArguments(Options))
            {
                if (useService)
                {
                    ServiceConvertFile(Options);
                }
                else
                {
                    ConvertFile(Options);
                }
            }
        }

        internal static string ServiceBaseAddress
        {
            get
            {
                return AppSetting("ServiceBaseAddress", "http://localhost:58998/");
            }
        }

        internal static string ServiceNetPipeAddress
        {
            get
            {
                return AppSetting("ServiceNetPipeAddress", "net.pipe://localhost/wwwplatform.convertpdf");
            }
        }

        internal static string AppSetting(string key, string defaultValue)
        {
            return ConfigurationManager.AppSettings[key] ?? defaultValue;
        }

        private static bool Exit(string message, int exitCode = 1)
        {
            log(message);
            Environment.ExitCode = exitCode;
            return false;
        }

        private static void log(string message, params object[] args)
        {
            log(string.Format(message, args));
        }

        private static void log(string message)
        {
            if (logfile != null)
            {
                byte[] bytes = Encoding.Default.GetBytes(message + "\r\n");
                logfile.Write(bytes, 0, bytes.Length);
            }
            if (!silent)
            {
                Console.WriteLine(message);
            }
        }
    }
}
