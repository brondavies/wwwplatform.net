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

        public static string FormatBytes(double bytes)
        {
            if (bytes >= 1100) //KB
            {
                if (bytes >= 1100000) //MB
                {
                    if (bytes >= 1100000000) //GB
                    {
                        if (bytes >= 1100000000000) //TB
                        {
                            return string.Format("{0} TB", Math.Round(bytes / 1099511627776, 2));
                        }
                        return string.Format("{0} GB", Math.Round(bytes / 1073741824, 2));
                    }
                    return string.Format("{0} MB", Math.Round(bytes / 1048576, 1));
                }
                return string.Format("{0} KB", Math.Round(bytes / 1024, 1));
            }
            return string.Format("{0} B", Math.Round(bytes));
        }
    }
}
