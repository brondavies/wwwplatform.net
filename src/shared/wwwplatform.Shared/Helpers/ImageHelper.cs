using System.IO;
using System.Linq;
using System.Web;

using FTTLib;

namespace wwwplatform.Shared.Helpers
{
    public class ImageHelper
    {
        public static bool IsImageFile(string filename)
        {
            return FTT.GetFileCategory(filename) == FileCategory.Image;
        }

        public static bool IsImageFile(HttpPostedFileBase file)
        {
            return IsImageFile(file.FileName);
        }
    }
}
