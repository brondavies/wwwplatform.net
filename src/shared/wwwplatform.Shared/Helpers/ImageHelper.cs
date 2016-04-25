using System.IO;
using System.Linq;
using System.Web;

namespace wwwplatform.Shared.Helpers
{
    public class ImageHelper
    {
        private static string[] valid_image_extensions
        {
            get
            {
                return new string[] { ".png", ".jpeg", ".jpg", ".bmp", ".gif", ".tif", ".tiff", ".pict", ".jp2" };
            }
        }

        public static bool IsImageFile(string extension)
        {
            return valid_image_extensions.Contains(extension.ToLower());
        }

        public static bool IsImageFile(HttpPostedFileBase file)
        {
            return IsImageFile(Path.GetExtension(file.FileName));
        }
    }
}
