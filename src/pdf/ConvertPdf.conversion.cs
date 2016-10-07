using ConvertPdf.Service;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Interop.Word;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.ServiceModel;

namespace ConvertPdf
{
    partial class ConvertPdf
    {
        internal static int ConvertFile(ConvertPdfOptions options)
        {
            int result = 0;
            string extension = Path.GetExtension(options.inputFile).ToLowerInvariant();
            log("Converting {0} ...", options.inputFile);
            try
            {
                if (File.Exists(options.outputFile))
                {
                    File.Delete(options.outputFile);
                }
                switch (extension)
                {
                    case ".doc":
                    case ".docx":
                    case ".dot":
                    case ".dotx":
                        ConvertWord(options);
                        break;
                    case ".pdf":
                        ConvertPdfToImage(options);
                        break;
                    case ".ppt":
                        ConvertPowerPoint(false, options);
                        break;
                    case ".pptm":
                    case ".pptx":
                        ConvertPowerPoint(true, options);
                        break;
                    case ".xls":
                    case ".xlsb":
                    case ".xlsx":
                        ConvertExcel(options);
                        break;
                }
            }
            catch (Exception ex)
            {
                log(ex.Message);
                result = -1;
            }
            return result;
        }

        private static void ServiceConvertFile(ConvertPdfOptions options)
        {
            System.ServiceModel.Channels.Binding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            using (ChannelFactory<IConvertPdfService> factory = new ChannelFactory<IConvertPdfService>(binding, ServiceNetPipeAddress))
            {
                //factory.Endpoint.Behaviors.Add(new Namedpi());
                IConvertPdfService server = factory.CreateChannel();

                Environment.ExitCode = server.Convert(options);
            }
        }

        private static int ServiceCheckRequirements()
        {
            int result = -1;
            System.ServiceModel.Channels.Binding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            using (ChannelFactory<IConvertPdfService> factory = new ChannelFactory<IConvertPdfService>(binding, ServiceNetPipeAddress))
            {
                //factory.Endpoint.Behaviors.Add(new Namedpi());
                IConvertPdfService server = factory.CreateChannel();

                result = server.Check();
            }
            return result;
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

        private static void ConvertExcel(ConvertPdfOptions options)
        {
            Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
            var excelSheet = appExcel.Workbooks.Open(options.inputFile);
            excelSheet.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF, options.outputFile, IgnorePrintAreas: true);
            excelSheet.Close(false);
            appExcel.ActiveWindow.Close(false);
        }

        private static void ConvertPowerPoint(bool as2007, ConvertPdfOptions options)
        {
            Microsoft.Office.Interop.PowerPoint.Application appPowerp = new Microsoft.Office.Interop.PowerPoint.Application();
            var powerpoint = as2007 ? appPowerp.Presentations.Open2007(options.inputFile) : appPowerp.Presentations.Open(options.inputFile);
            powerpoint.ExportAsFixedFormat(options.outputFile, PpFixedFormatType.ppFixedFormatTypePDF);
            appPowerp.ActiveWindow.Close();
        }

        private static void ConvertWord(ConvertPdfOptions options)
        {
            Microsoft.Office.Interop.Word.Application appWord = new Microsoft.Office.Interop.Word.Application();
            var wordDocument = appWord.Documents.Open(options.inputFile);
            wordDocument.ExportAsFixedFormat(options.outputFile, WdExportFormat.wdExportFormatPDF);
            wordDocument.Close(false);
            appWord.ActiveWindow.Close(false);
        }

        private static void ConvertPdfToImage(ConvertPdfOptions options)
        {
            string dll = GetGhostScriptDllPath();

            GhostscriptVersionInfo version = new GhostscriptVersionInfo(new System.Version(0, 0, 0), dll, string.Empty, GhostscriptLicense.GPL);
            using (GhostscriptRasterizer gs = new GhostscriptRasterizer())
            {
                gs.Open(options.inputFile, version, false);
                if (gs.PageCount > 0)
                {
                    int dpi = options.quality * 3;
                    using (Image image = gs.GetPage(dpi, dpi, options.pageNumber))
                    {
                        int imageWidth = image.Width;
                        if (options.maxWidth > 0 && options.maxWidth < imageWidth)
                        {
                            double ratio = (double)options.maxWidth / imageWidth;
                            int maxHeight = Convert.ToInt32(Math.Round(ratio * image.Height));
                            using (Image thumb = ResizeImage(image, options.maxWidth, maxHeight))
                            {
                                thumb.Save(options.outputFile);
                            }
                        }
                        else
                        {
                            image.Save(options.outputFile);
                        }
                    }
                }
                gs.Close();
            }
        }
    }
}
