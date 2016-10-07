using System;
using System.IO;
using System.Linq;

namespace ConvertPdf
{
    partial class ConvertPdf
    {
        private static void ParseArguments(string[] args, ConvertPdfOptions options)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg.ToLowerInvariant())
                {
                    case "-check":
                    case "/check":
                    case "-test":
                    case "/test":
                        checkSupport = true;
                        break;
                    case "/f":
                    case "-f":
                    case "/force":
                    case "-force":
                    case "/overwrite":
                    case "-overwrite":
                        overwrite = true;
                        break;
                    case "-i":
                    case "/i":
                    case "-install":
                    case "/install":
                        installService = true;
                        break;
                    case "-log":
                    case "/log":
                        i++; arg = args[i];
                        logfile = File.OpenWrite(arg);
                        break;
                    case "/p":
                    case "-p":
                    case "/page":
                    case "-page":
                        i++; arg = args[i];
                        options.pageNumber = int.Parse(arg);
                        break;
                    case "/q":
                    case "-q":
                    case "/quality":
                    case "-quality":
                        i++; arg = args[i];
                        options.quality = int.Parse(arg);
                        break;
                    case "-r":
                    case "/r":
                    case "-remote":
                    case "/remote":
                        useService = true;
                        break;
                    case "-service":
                    case "/service":
                        runService = true;
                        break;
                    case "-s":
                    case "/s":
                    case "-silent":
                    case "/silent":
                        silent = true;
                        break;
                    case "-u":
                    case "/u":
                    case "-uninstall":
                    case "/uninstall":
                        uninstallService = true;
                        break;
                    case "/w":
                    case "-w":
                    case "/width":
                    case "-width":
                        i++; arg = args[i];
                        options.maxWidth = int.Parse(arg);
                        break;
                    default:
                        if (options.inputFile == null)
                        {
                            options.inputFile = Path.GetFullPath(arg);
                            if (IsPdfFile(options.inputFile))
                            {
                                options.outputFile = Path.ChangeExtension(options.inputFile, ".jpg");
                            }
                            else
                            {
                                options.outputFile = Path.ChangeExtension(options.inputFile, ".pdf");
                            }
                        }
                        else
                        {
                            options.outputFile = Path.GetFullPath(arg);
                        }
                        break;
                }
            }
        }

        private static bool ValidateArguments(ConvertPdfOptions options)
        {
            if (options.inputFile == null)
            {
                return Exit("Input file is missing");
            }
            else if (!IsSupportedType(options.inputFile))
            {
                return Exit("Unsupported file type: " + Path.GetExtension(options.inputFile));
            }
            else if (options.outputFile == null)
            {
                return Exit("Output file is missing");
            }
            else if (File.Exists(options.outputFile) && !overwrite)
            {
                return Exit("Output file exists - not overwriting " + options.outputFile);
            }
            return true;
        }

        private static bool IsSupportedType(string filename)
        {
            string extension = Path.GetExtension(filename).ToLowerInvariant();
            return supportedFileTypes.Contains(extension);
        }

        private static bool IsPdfFile(string filename)
        {
            string extension = Path.GetExtension(filename).ToLowerInvariant();
            return extension == ".pdf";
        }

        internal static int CheckRequirements()
        {
            int result = 0;
            try
            {
                if (useService)
                {
                    result = ServiceCheckRequirements();
                }
                else
                {
                    result =
                        CanReadDocuments() |
                        CanReadPowerpoint() |
                        CanReadSpreadsheets() |
                        CanReadPdf();
                }
            }
            catch (Exception ex)
            {
                log(ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }

        private static int CanReadDocuments()
        {
            int result = 0;
            try
            {
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                result = WORD_INSTALLED;
                log("Microsoft Word\t\tOK");
            }
            catch (Exception e)
            {
                log("Microsoft Word\t\tFAILED");
                log("" + e.Message + "\r\n" + e.InnerException?.Message);
            }
            return result;
        }

        private static int CanReadPowerpoint()
        {
            int result = 0;
            try
            {
                Microsoft.Office.Interop.PowerPoint.Application appPowerp = new Microsoft.Office.Interop.PowerPoint.Application();
                result = PPT_INSTALLED;
                log("Microsoft Powerpoint\tOK");
            }
            catch (Exception e)
            {
                log("Microsoft Powerpoint\tFAILED");
                log("" + e.Message + "\r\n" + e.InnerException?.Message);
            }
            return result;
        }

        private static int CanReadSpreadsheets()
        {
            int result = 0;
            try
            {
                Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
                result = EXCEL_INSTALLED;
                log("Microsoft Excel\t\tOK");
            }
            catch (Exception e)
            {
                log("Microsoft Excel\t\tFAILED");
                log("" + e.Message + "\r\n" + e.InnerException?.Message);
            }
            return result;
        }

        private static int CanReadPdf()
        {
            int result = 0;
            if (File.Exists(GetGhostScriptDllPath()))
            {
                log("GhostScript\t\tOK");
                result = GS_INSTALLED;
            }
            else
            {
                log("GhostScript\t\tFAILED");
            }
            return result;
        }
    }
}
