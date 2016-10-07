using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ConvertPdf.Service
{
    [ServiceContract(Namespace = "http://wwwplatform.convertpdf")]
    interface IConvertPdfService
    {
        [OperationContract]
        int Convert(ConvertPdfOptions options);

        [OperationContract]
        int Check();
    }
}
