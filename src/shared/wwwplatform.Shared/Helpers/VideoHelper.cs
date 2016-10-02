using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using FTTLib;

namespace wwwplatform.Shared.Helpers
{
    public class VideoHelper
    {
        public static bool IsVideoFile(string filename)
        {
            return FTT.GetFileCategory(filename) == FileCategory.Video;
        }

        public static bool IsVideoFile(HttpPostedFileBase file)
        {
            return IsVideoFile(file.FileName);
        }

        public static bool IsAudioFile(HttpPostedFileBase file)
        {
            return IsAudioFile(file.FileName);
        }

        public static bool IsAudioFile(string filename)
        {
            return FTT.GetFileCategory(filename) == FileCategory.Audio;
        }
    }
}
