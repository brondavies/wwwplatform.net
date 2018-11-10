using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using FTTLib;
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
            return string.Format("http://{0}/Downloads/{1}/preview", settings.CanonicalHostName, Id);
        }

        public string GetFileName()
        {
            return Name.CleanFileName() + System.IO.Path.GetExtension(Location);
        }

        public string OriginalFileName
        {
            get
            {
                return Name + System.IO.Path.GetExtension(Location);
            }
        }

        public string GetMimeType()
        {
            return FTT.GetMimeType(Location);
        }

        public FileType GetFileType()
        {
            if (string.IsNullOrEmpty(Location))
            {
                return FileType.Unknown;
            }
            var category = FTT.GetFileCategory(Location);
            switch (category)
            {
                case FileCategory.Archive: return FileType.Archive;

                case FileCategory.Audio: return FileType.Audio;

                case FileCategory.Code: return FileType.Code;

                case FileCategory.Document: return FileType.Document;

                case FileCategory.Image: return FileType.Image;

                case FileCategory.Presentation: return FileType.Presentation;

                case FileCategory.PDF: return FileType.PDF;

                case FileCategory.Spreadsheet: return FileType.Spreadsheet;

                case FileCategory.Video: return FileType.Video;

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