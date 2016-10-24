using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Models.ViewModels
{
    public class UploadResults
    {
        public static int Failed = 0;
        public static int OK = 1;

        public int status = 0;
        public string message;
        public List<WebFile> files = new List<WebFile>();
    }
}
