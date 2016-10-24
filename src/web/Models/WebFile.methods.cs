using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using wwwplatform.Shared.Extensions.System;
using wwwplatform.Shared.Extensions;

namespace wwwplatform.Models
{
    public partial class WebFile : Auditable, Permissible
    {
        internal static IQueryable<WebFile> GetAvailableFiles(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager, bool owner = false)
        {
            var files = Permission.GetPermissible<WebFile>(db, User, UserManager, RoleManager);
            if (owner)
            {
                string UserName = User.Identity.Name;
                files = files.Where(f => f.UpdatedBy == UserName);
            }
            return files.OrderBy(f => f.Name);
        }

        public bool IsOwner(IPrincipal User)
        {
            return UpdatedBy == User.Identity.Name;
        }

        public void Update(WebFile update, int? timeZoneOffset)
        {
            if (!string.IsNullOrEmpty(update.Name))
            {
                Name = update.Name;
            }
            Description = update.Description;
            DisplayDate = update.DisplayDate;
            if (timeZoneOffset.HasValue && DisplayDate.HasValue)
            {
                DisplayDate = DisplayDate.Value.FromTimezone(timeZoneOffset.Value);
            }
        }

        [NotMapped]
        public string Icon
        {
            get
            {
                switch (GetFileType())
                {
                    case FileType.Code:
                        return "fa-file-code-o";
                    case FileType.Video:
                        return "fa-file-video-o";
                    case FileType.Audio:
                        return "fa-file-audio-o";
                    case FileType.Archive:
                        return "fa-file-archive-o";
                    case FileType.Image:
                        return "fa-file-image-o";
                    case FileType.Presentation:
                        return "fa-file-powerpoint-o";
                    case FileType.Spreadsheet:
                        return "fa-file-excel-o";
                    case FileType.Document:
                        return "fa-file-word-o";
                    case FileType.PDF:
                        return "fa-file-pdf-o";
                    case FileType.Text:
                        return "fa-file-text-o";
                    default:
                        return "fa-file-text";
                }
            }
        }

        public string GetUrl(Settings settings)
        {
            return string.Format("https://{0}/WebFiles/Details/{1}", settings.CanonicalHostName, Id);
        }

        public string GetPreviewUrl(Settings settings)
        {
            return string.Format("https://{0}/WebFiles/Details/{1}/preview", settings.CanonicalHostName, Id);
        }

        public string GetFileName()
        {
            return Name.CleanFileName() + System.IO.Path.GetExtension(Location);
        }

        public FileType GetFileType()
        {
            if (string.IsNullOrEmpty(Location))
            {
                return FileType.Unknown;
            }
            string ext = System.IO.Path.GetExtension(Location).ToLower();
            switch (ext)
            {
                case ".7z":
                case ".7zip":
                case ".ace":
                case ".air":
                case ".apk":
                case ".appxbundle":
                case ".arc":
                case ".arj":
                case ".asec":
                case ".bar":
                case ".bz2":
                case ".bzip":
                case ".cab":
                case ".cso":
                case ".deb":
                case ".dlc":
                case ".dmg":
                case ".gz":
                case ".gzip":
                case ".hqx":
                case ".inv":
                case ".ipa":
                case ".iso":
                case ".isz":
                case ".jar":
                case ".msu":
                case ".nbh":
                case ".pak":
                case ".rar":
                case ".rpm":
                case ".sis":
                case ".sisx":
                case ".sit":
                case ".sitd":
                case ".sitx":
                case ".tar":
                case ".tar.gz":
                case ".tgz":
                case ".webarchive":
                case ".xap":
                case ".z":
                case ".zip":
                    return FileType.Archive;

                case ".3ga":
                case ".aac":
                case ".aiff":
                case ".amr":
                case ".ape":
                case ".arf":
                case ".asf":
                case ".asx":
                case ".cda":
                case ".dvf":
                case ".flac":
                case ".gp4":
                case ".gp5":
                case ".gpx":
                case ".logic":
                case ".m4a":
                case ".m4b":
                case ".m4p":
                case ".midi":
                case ".mp3":
                case ".ogg":
                case ".pcm":
                case ".rec":
                case ".snd":
                case ".sng":
                case ".uax":
                case ".wav":
                case ".wma":
                case ".wpl":
                case ".zab":
                    return FileType.Audio;

                case ".as":
                case ".asm":
                case ".asp":
                case ".aspx":
                case ".bat":
                case ".c":
                case ".cp":
                case ".cpp":
                case ".cs":
                case ".css":
                case ".gradle":
                case ".htm":
                case ".inc":
                case ".jad":
                case ".java":
                case ".js":
                case ".json":
                case ".jsp":
                case ".lib":
                case ".m":
                case ".matlab":
                case ".ml":
                case ".o":
                case ".perl":
                case ".php":
                case ".pl":
                case ".ps1":
                case ".py":
                case ".rb":
                case ".rc":
                case ".rss":
                case ".scpt":
                case ".sh":
                case ".sql":
                case ".src":
                case ".swift":
                case ".vb":
                case ".vbs":
                case ".ws":
                case ".xaml":
                case ".xcodeproj":
                case ".xml":
                case ".xsd":
                case ".xsl":
                case ".xslt":
                case ".yml":
                    return FileType.Code;

                case ".abw":
                case ".aww":
                case ".azw":
                case ".azw3":
                case ".azw4":
                case ".cbr":
                case ".cbz":
                case ".chm":
                case ".cnt":
                case ".dbx":
                case ".djvu":
                case ".doc":
                case ".docm":
                case ".docx":
                case ".dot":
                case ".dotm":
                case ".dotx":
                case ".epub":
                case ".fb2":
                case ".iba":
                case ".ibooks":
                case ".ind":
                case ".indd":
                case ".key":
                case ".keynote":
                case ".lit":
                case ".mht":
                case ".mobi":
                case ".mpp":
                case ".odf":
                case ".odt":
                case ".ott":
                case ".pages":
                case ".pmd":
                case ".prn":
                case ".prproj":
                case ".ps":
                case ".pub":
                case ".pwi":
                case ".rep":
                case ".rtf":
                case ".sdd":
                case ".sdw":
                case ".shs":
                case ".snp":
                case ".sxw":
                case ".tpl":
                case ".vsd":
                case ".wlmp":
                case ".wpd":
                case ".wps":
                case ".wri":
                    return FileType.Document;

                case ".bmp":
                case ".cpt":
                case ".dds":
                case ".dib":
                case ".dng":
                case ".dt2":
                case ".emf":
                case ".gif":
                case ".ico":
                case ".icon":
                case ".jpeg":
                case ".jpg":
                case ".pcx":
                case ".pic":
                case ".png":
                case ".psd":
                case ".psdx":
                case ".raw":
                case ".tga":
                case ".thm":
                case ".tif":
                case ".tiff":
                case ".wbmp":
                case ".wdp":
                case ".webp":
                    return FileType.Image;

                case ".pot":
                case ".potx":
                case ".pps":
                case ".ppsx":
                case ".ppt":
                case ".pptm":
                case ".pptx":
                    return FileType.Presentation;

                case ".oxps":
                case ".pdf":
                case ".xps":
                    return FileType.PDF;

                case ".ods":
                case ".numbers":
                case ".sdc":
                case ".xls":
                case ".xlsx":
                case ".xlsb":
                    return FileType.Spreadsheet;

                case ".264":
                case ".3g2":
                case ".3gp":
                case ".avi":
                case ".bik":
                case ".dash":
                case ".dat":
                case ".dvr":
                case ".flv":
                case ".h264":
                case ".m2t":
                case ".m2ts":
                case ".m4v":
                case ".mkv":
                case ".mod":
                case ".mov":
                case ".mp4":
                case ".mpeg":
                case ".mpg":
                case ".mswmm":
                case ".mts":
                case ".ogv":
                case ".rmvb":
                case ".swf":
                case ".tod":
                case ".tp":
                case ".ts":
                case ".vob":
                case ".webm":
                case ".wmv":
                    return FileType.Video;

                default:
                    return FileType.Text;
            }
        }
    }

    public enum FileType
    {
        Archive,
        Audio,
        Code,
        Document,
        Image,
        PDF,
        Presentation,
        Spreadsheet,
        Text,
        Video,
        Unknown
    }
}