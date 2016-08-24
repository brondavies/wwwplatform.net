using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using wwwplatform.Models;
using wwwplatform.Shared.Extensions.System;

namespace wwwplatform.Extensions.Helpers
{
    public class FileStorage
    {
        public static string Save(FileInfo file, HttpContextBase context)
        {
            string date  = DateTime.UtcNow.ToString("yyyyMMdd");
            string folder = Path.Combine(Path.GetFullPath(Settings.Create(context).UserFilesDir), date);
            Directory.CreateDirectory(folder);
            string newfile = file.CopyTo(Path.Combine(folder, file.Name)).FullName;
            return "/" + newfile.ToAppPath(context);
        }
    }
}
