using ConvertPdf.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;

namespace ConvertPdf
{
    partial class ConvertPdf
    {
        const string ServiceName = "ConvertPdfService";

        private static string System32 { get { return Environment.GetFolderPath(Environment.SpecialFolder.SystemX86); } }

        private static void Install()
        {
            string ExecutableName = Assembly.GetExecutingAssembly().Location;
            string sc = Path.Combine(System32, "sc.exe");
            Uninstall();
            string install = string.Format("create \"{0}\" binPath= \"{1} /service\" DisplayName= \"{2}\" start= auto", ServiceName, ExecutableName, "PDF Conversion Service");
            RunAndWait(sc, install);
        }
        
        private static void StartService()
        {
            ConvertPdfWindowsService service = new ConvertPdfWindowsService();
            ServiceBase.Run(service);
        }

        private static void Uninstall()
        {
            string sc = Path.Combine(System32, "sc.exe");
            RunAndWait(sc, "stop " + ServiceName);
            RunAndWait(sc, "delete " + ServiceName);
        }

        public static int RunAndWait(string exe, string arguments = "", bool shellExecute = false, bool wait = true, string verb = null, bool hidewindow = false)
        {
            log("Running {0} {1}", exe, arguments);
            Process process = new Process();
            process.StartInfo.FileName = exe;
            if (string.IsNullOrEmpty(arguments) == false)
            {
                process.StartInfo.Arguments = arguments;
            }
            process.StartInfo.UseShellExecute = shellExecute;
            process.StartInfo.CreateNoWindow = hidewindow;
            if (string.IsNullOrEmpty(verb) == false)
            {
                process.StartInfo.Verb = verb;
            }
            bool started = process.Start();

            if (started && wait)
            {
                process.WaitForExit();
                return process.ExitCode;
            }

            return process.Id;
        }
    }
}
