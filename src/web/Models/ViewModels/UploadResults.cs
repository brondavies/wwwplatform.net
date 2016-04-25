using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models.ViewModels
{
    public class UploadResults
    {
        public static int OK = 0;
        public static int Failed = 1;

        public int status = 0;
        public string message;
        public WebFile file;
    }
}
