using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;

namespace ConvertPdf
{
    class ConvertPdf
    {
        private static string inputFile;
        private static string outputFile;
        private static bool overwrite;
        private static bool checkSupport;
        private static int maxWidth = 0;
        private static int quality = 70;
        private static int pageNumber = 1;
        private static string[] supportedFileTypes = new string[] {
            ".doc", ".docx", ".dot", ".dotx", ".pdf", ".ppt", ".pptm", ".pptx", ".xls", ".xlsb", ".xlsx"
        };

        const int WORD_INSTALLED = 1;
        const int PPT_INSTALLED = 2;
        const int EXCEL_INSTALLED = 4;
        const int GS_INSTALLED = 8;

        static void Main(string[] args)
        {
            ParseArguments(args);
            if (checkSupport)
            {
                CheckRequirements();
                return;
            }
            if (ValidateArguments())
            {
                ConvertFile();
            }
        }

        private static void CheckRequirements()
        {
            Environment.ExitCode =
                CanReadDocuments() |
                CanReadPowerpoint() |
                CanReadSpreadsheets() |
                CanReadPdf();
        }

        private static int CanReadDocuments()
        {
            int result = 0;
            try
            {
                Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
                appWord.Quit(false);
                result = WORD_INSTALLED;
                log("Microsoft Word\t\tOK");
            }
            catch(Exception e)
            {
                log(e.Message);
                log("Microsoft Word\t\tFAILED");
            }
            return result;
        }

        private static int CanReadPowerpoint()
        {
            int result = 0;
            try
            {
                Microsoft.Office.Interop.PowerPoint.Application appPowerp = new Microsoft.Office.Interop.PowerPoint.Application();
                appPowerp.Quit();
                result = PPT_INSTALLED;
                log("Microsoft Powerpoint\tOK");
            }
            catch (Exception e)
            {
                log(e.Message);
                log("Microsoft Powerpoint\tFAILED");
            }
            return result;
        }

        private static int CanReadSpreadsheets()
        {
            int result = 0;
            try
            {
                Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
                appExcel.Quit();
                result = EXCEL_INSTALLED;
                log("Microsoft Excel\t\tOK");
            }
            catch (Exception e)
            {
                log(e.Message);
                log("Microsoft Excel\t\tFAILED");
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

        private static void ConvertFile()
        {
            string extension = Path.GetExtension(inputFile).ToLowerInvariant();
            log("Converting {0} ...", inputFile);
            try
            {
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                switch (extension)
                {
                    case ".doc":
                    case ".docx":
                    case ".dot":
                    case ".dotx":
                        ConvertWord();
                        break;
                    case ".pdf":
                        ConvertPdfToImage();
                        break;
                    case ".ppt":
                        ConvertPowerPoint(false);
                        break;
                    case ".pptm":
                    case ".pptx":
                        ConvertPowerPoint(true);
                        break;
                    case ".xls":
                    case ".xlsb":
                    case ".xlsx":
                        ConvertExcel();
                        break;
                }
            }
            catch (Exception ex)
            {
                Exit(ex.Message, -1);
            }
        }


        private static void ConvertPdfToImage()
        {
            string dll = GetGhostScriptDllPath();

            GhostscriptVersionInfo version = new Ghostscript.NET.GhostscriptVersionInfo(new System.Version(0, 0, 0), dll, string.Empty, Ghostscript.NET.GhostscriptLicense.GPL);
            using (GhostscriptRasterizer gs = new GhostscriptRasterizer())
            {
                gs.Open(inputFile, version, false);
                if (gs.PageCount > 0)
                {
                    int dpi = quality * 3;
                    using (System.Drawing.Image image = gs.GetPage(dpi, dpi, pageNumber))
                    {
                        int imageWidth = image.Width;
                        if (maxWidth > 0 && maxWidth < imageWidth)
                        {
                            double ratio = (double)maxWidth / (double)imageWidth;
                            int maxHeight = Convert.ToInt32(Math.Round(ratio * image.Height));
                            using (Image thumb = ResizeImage(image, maxWidth, maxHeight))
                            {
                                thumb.Save(outputFile);
                            }
                        }
                        else
                        {
                            image.Save(outputFile);
                        }
                    }
                }
                gs.Close();
            }
        }

        private static string GetGhostScriptDllPath()
        {
            string appdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dll = Environment.Is64BitProcess ? "gsdll64.dll" : "gsdll32.dll";
            return Path.Combine(appdir, dll);
        }

        private static Image ResizeImage(Image image, int width, int height)
        {
            return new Bitmap(image, new Size(width, height));
        }

        private static void ConvertExcel()
        {
            Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
            var excelSheet = appExcel.Workbooks.Open(inputFile);
            excelSheet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, outputFile, IgnorePrintAreas: true);
            excelSheet.Close(false);
            appExcel.ActiveWindow.Close(false);
            appExcel.Quit();
        }

        private static void ConvertPowerPoint(bool as2007)
        {
            Microsoft.Office.Interop.PowerPoint.Application appPowerp = new Microsoft.Office.Interop.PowerPoint.Application();
            var powerpoint = as2007 ? appPowerp.Presentations.Open2007(inputFile) : appPowerp.Presentations.Open(inputFile);
            powerpoint.ExportAsFixedFormat(outputFile, PpFixedFormatType.ppFixedFormatTypePDF);
            appPowerp.ActiveWindow.Close();
            appPowerp.Quit();
        }

        private static void ConvertWord()
        {
            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = appWord.Documents.Open(inputFile);
            wordDocument.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatPDF);
            wordDocument.Close(false);
            appWord.ActiveWindow.Close(false);
            appWord.Quit(false);
        }

        private static void ParseArguments(string[] args)
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
                    case "/p":
                    case "-p":
                    case "/page":
                    case "-page":
                        i++; arg = args[i];
                        pageNumber = int.Parse(arg);
                        break;
                    case "/q":
                    case "-q":
                    case "/quality":
                    case "-quality":
                        i++; arg = args[i];
                        quality = int.Parse(arg);
                        break;
                    case "/w":
                    case "-w":
                    case "/width":
                    case "-width":
                        i++; arg = args[i];
                        maxWidth = int.Parse(arg);
                        break;
                    default:
                        if (inputFile == null)
                        {
                            inputFile = Path.GetFullPath(arg);
                            if (IsPdfFile(inputFile))
                            {
                                outputFile = Path.ChangeExtension(inputFile, ".jpg");
                            }
                            else
                            {
                                outputFile = Path.ChangeExtension(inputFile, ".pdf");
                            }
                        }
                        else
                        {
                            outputFile = Path.GetFullPath(arg);
                        }
                        break;
                }
            }
        }

        private static bool ValidateArguments()
        {
            if (inputFile == null)
            {
                return Exit("Input file is missing");
            }
            else if (!IsSupportedType(inputFile))
            {
                return Exit("Unsupported file type: " + Path.GetExtension(inputFile));
            }
            else if (outputFile == null)
            {
                return Exit("Output file is missing");
            }
            else if (File.Exists(outputFile) && !overwrite)
            {
                return Exit("Output file exists - not overwriting " + outputFile);
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

        private static bool Exit(string message, int exitCode = 1)
        {
            log(message);
            Environment.ExitCode = exitCode;
            return false;
        }

        private static void log(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }
    }
}
