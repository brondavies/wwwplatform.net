using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wwwplatform.Shared.Helpers
{
    public class ContentTypeHelper
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public static string GetContentType(string filename)
        {
            string contenttype = "application/octet-stream";
            string filetype = filename.Split('.').Last().ToLower();
            switch (filetype)
            {
                case "png":
                    contenttype = "image/png";
                    break;
                case "jp2":
                case "jpg":
                case "jpeg":
                    contenttype = "image/jpeg";
                    break;
                case "bmp":
                    contenttype = "image/bmp";
                    break;
                case "gif":
                    contenttype = "image/gif";
                    break;
                case "tif":
                case "tiff":
                    contenttype = "image/tiff";
                    break;
                case "pic":
                case "pict":
                    contenttype = "image/pict";
                    break;
                case "3g2":
                    contenttype = "application/3gpp2";
                    break;
                case "3gp":
                case "3gpp":
                    contenttype = "video/3gpp";
                    break;
                case "asf":
                    contenttype = "video/x-ms-asf";
                    break;
                case "avi":
                    contenttype = "video/x-msvideo";
                    break;
                case "divx":
                    contenttype = "video/divx";
                    break;
                case "f4v":
                    contenttype = "video/x-f4v";
                    break;
                case "flv":
                    contenttype = "video/x-flv";
                    break;
                case "h264":
                    contenttype = "video/h264";
                    break;
                case "ifo":
                    contenttype = "video/x-ifo";
                    break;
                case "m2ts":
                    contenttype = "video/m2ts";
                    break;
                case "m4v":
                    contenttype = "video/x-m4v";
                    break;
                case "mkv":
                    contenttype = "video/x-mkv";
                    break;
                case "mod":
                    contenttype = "video/x-mod";
                    break;
                case "mov":
                    contenttype = "video/quicktime";
                    break;
                case "mp4":
                    contenttype = "video/mp4";
                    break;
                case "mpg":
                case "mpeg":
                    contenttype = "video/mpeg";
                    break;
                case "mswmm":
                    contenttype = "video/x-mswmm";
                    break;
                case "mts":
                    contenttype = "model/vnd.mts";
                    break;
                case "mxf":
                    contenttype = "application/mxf";
                    break;
                case "ogv":
                    contenttype = "video/ogg";
                    break;
                case "rm":
                    contenttype = "application/vnd.rn-realmedia";
                    break;
                case "swf":
                    contenttype = "application/x-shockwave-flash";
                    break;
                case "ts":
                    contenttype = "video/ts";
                    break;
                case "vep":
                    contenttype = "video/x-vep";
                    break;
                case "vob":
                    contenttype = "video/x-vob";
                    break;
                case "webm":
                    contenttype = "video/webm";
                    break;
                case "wlmp":
                    contenttype = "application/wlmoviemaker";
                    break;
                case "wmv":
                    contenttype = "video/x-ms-wmv";
                    break;
                case "3ga":
                    contenttype = "audio/3ga";
                    break;
                case "aac":
                    contenttype = "audio/aac";
                    break;
                case "aif":
                case "aiff":
                    contenttype = "audio/aiff";
                    break;
                case "amr":
                    contenttype = "audio/amr";
                    break;
                case "au":
                case "aup":
                    contenttype = "audio/basic";
                    break;
                case "caf":
                    contenttype = "audio/x-caf";
                    break;
                case "flac":
                    contenttype = "audio/x-flac";
                    break;
                case "gsm":
                    contenttype = "audio/x-gsm";
                    break;
                case "kar":
                    contenttype = "audio/x-kar";
                    break;
                case "m4a":
                    contenttype = "audio/m4a";
                    break;
                case "m4p":
                    contenttype = "audio/m4p";
                    break;
                case "m4r":
                    contenttype = "audio/x-m4r";
                    break;
                case "mmf":
                    contenttype = "application/x-smaf";
                    break;
                case "mp2":
                    contenttype = "audio/mpeg";
                    break;
                case "mp3":
                    contenttype = "audio/mpeg";
                    break;
                case "mpga":
                    contenttype = "audio/mpeg";
                    break;
                case "ogg":
                    contenttype = "audio/ogg";
                    break;
                case "oma":
                    contenttype = "application/x-oma";
                    break;
                case "opus":
                    contenttype = "application/x-opus";
                    break;
                case "qcp":
                    contenttype = "application/x-qcp";
                    break;
                case "ra":
                case "ram":
                    contenttype = "audio/x-pn-realaudio";
                    break;
                case "wav":
                    contenttype = "audio/wav";
                    break;
                case "wma":
                    contenttype = "audio/x-ms-wma";
                    break;
                case "xspf":
                    contenttype = "application/xspf";
                    break;
                case "doc":
                    contenttype = "application/msword";
                    break;
                case "docx":
                    contenttype = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "pdf":
                    contenttype = "application/pdf";
                    break;
                case "xls":
                    contenttype = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    contenttype = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "ppt":
                    contenttype = "application/vnd.ms-powerpoint";
                    break;
                case "pptx":
                    contenttype = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    break;
                case "rtf":
                    contenttype = "application/rtf";
                    break;
                case "txt":
                    contenttype = "text/plain";
                    break;
                case "wps":
                    contenttype = "application/vnd.ms-works";
                    break;
                case "xps":
                    contenttype = "application/vnd.ms-xpsdocument";
                    break;
                case "epub":
                    contenttype = "application/epub";
                    break;
                case "ppsx":
                    contenttype = "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                    break;
                case "ods":
                    contenttype = "application/vnd.oasis.opendocument.spreadsheet";
                    break;
                case "csv":
                    contenttype = "text/csv";
                    break;
            }
            return contenttype;
        }
    }
}
