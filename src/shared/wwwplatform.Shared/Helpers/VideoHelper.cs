using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace wwwplatform.Shared.Helpers
{
    public class VideoHelper
    {
        private static string[] valid_video_extensions
        {
            get
            {
                return new string[] {
                ".3g2", ".3gp", ".3gpp", ".asf", ".avi", ".divx", ".f4v", ".flv", ".h264", ".ifo", ".m2ts", ".m4v", ".mkv", ".mod", ".mov", ".mp4", ".mpeg", ".mpg", ".mswmm", ".mts", ".mxf", ".ogv", ".rm", ".swf", ".ts", ".vep", ".vob", ".webm", ".wlmp", ".wmv"
                };
            }
        }

        private static string[] valid_audio_extensions
        {
            get
            {
                return new string[] {
                ".3ga", ".aac", ".aif", ".aiff", ".amr", ".au", ".aup", ".caf", ".flac", ".gsm", ".kar", ".m4a", ".m4p", ".m4r", ".mmf", ".mp2", ".mp3", ".mpga", ".ogg", ".oma", ".opus", ".qcp", ".ra", ".ram", ".wav", ".wma", ".xspf"
                };
            }
        }

        public static bool IsVideoFile(string extension)
        {
            return valid_video_extensions.Contains(extension.ToLower());
        }

        public static bool IsVideoFile(HttpPostedFileBase file)
        {
            return IsVideoFile(Path.GetExtension(file.FileName));
        }

        public static bool IsAudioFile(HttpPostedFileBase file)
        {
            return IsAudioFile(Path.GetExtension(file.FileName));
        }

        public static bool IsAudioFile(string extension)
        {
            return valid_audio_extensions.Contains(extension.ToLower());
        }
    }
}
