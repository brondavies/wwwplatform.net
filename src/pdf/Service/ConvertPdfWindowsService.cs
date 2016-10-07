using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ConvertPdf.Service
{
    partial class ConvertPdfWindowsService : ServiceBase
    {
        public ConvertPdfWindowsService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ConvertPdfService.Start();
        }

        protected override void OnStop()
        {
            ConvertPdfService.Stop();
        }
    }
}
