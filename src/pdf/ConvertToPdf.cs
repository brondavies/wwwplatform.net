using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConvertToPdf
{
    class ConvertToPdf
    {
        private static string inputFile;
        private static string outputFile;
        private static bool overwrite;
        private static string[] supportedFileTypes = new string[] {
            ".doc", ".docx", ".dot", ".dotx", ".ppt", ".pptm", ".pptx", ".xls", ".xlsb", ".xlsx"
        };

        static void Main(string[] args)
        {
            ParseArguments(args);
            if (ValidateArguments())
            {
                ConvertFile();
            }
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

        private static void ConvertExcel()
        {
            Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
            var excelSheet = appExcel.Workbooks.Open(inputFile);
            excelSheet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, outputFile, IgnorePrintAreas: true);
            excelSheet.Close(false);
            appExcel.ActiveWindow.Close(false);
        }

        private static void ConvertPowerPoint(bool as2007)
        {
            Microsoft.Office.Interop.PowerPoint.Application appPowerp = new Microsoft.Office.Interop.PowerPoint.Application();
            var powerpoint = as2007 ? appPowerp.Presentations.Open2007(inputFile) : appPowerp.Presentations.Open(inputFile);
            powerpoint.ExportAsFixedFormat(outputFile, PpFixedFormatType.ppFixedFormatTypePDF);
            appPowerp.ActiveWindow.Close();
        }

        private static void ConvertWord()
        {
            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = appWord.Documents.Open(inputFile);
            wordDocument.ExportAsFixedFormat(outputFile, WdExportFormat.wdExportFormatPDF);
            wordDocument.Close(false);
            appWord.ActiveWindow.Close(false);
        }

        private static void ParseArguments(string[] args)
        {
            foreach (string arg in args)
            {
                switch (arg.ToLowerInvariant())
                {
                    case "/f":
                    case "-f":
                    case "/force":
                    case "-force":
                    case "/overwrite":
                    case "-overwrite":
                        overwrite = true;
                        break;
                    default:
                        if (inputFile == null)
                        {
                            inputFile = Path.GetFullPath(arg);
                            outputFile = Path.ChangeExtension(inputFile, ".pdf");
                        }
                        else if (arg.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
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
