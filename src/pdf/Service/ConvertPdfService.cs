using System;
using System.ServiceModel;

namespace ConvertPdf.Service
{
    public class ConvertPdfService : IConvertPdfService
    {
        public int Convert(ConvertPdfOptions options)
        {
            return ConvertPdf.ConvertFile(options);
        }

        public int Check()
        {
            return ConvertPdf.CheckRequirements();
        }

        private static ServiceHost serviceHost;

        internal static void Start()
        {
            if (serviceHost == null)
            {
                Uri baseAddress = new Uri(ConvertPdf.ServiceBaseAddress);
                string address = ConvertPdf.ServiceNetPipeAddress;

                serviceHost = new ServiceHost(typeof(ConvertPdfService), baseAddress);
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                serviceHost.AddServiceEndpoint(typeof(IConvertPdfService), binding, address);

                serviceHost.Open();
            }
        }

        internal static void Stop()
        {
            try
            {
                serviceHost?.Close();
                serviceHost = null;
            }
            catch { }
        }
    }
}
