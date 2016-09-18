using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

using Microsoft.AspNet.Identity;

namespace wwwplatform.Models
{
    public partial class WebFile : Auditable, Permissible
    {
        internal static IQueryable<WebFile> GetAvailableFiles(ApplicationDbContext db, IPrincipal User, ApplicationUserManager UserManager, ApplicationRoleManager RoleManager)
        {
            var files = Permission.GetPermissible<WebFile>(db, User, UserManager, RoleManager);
            return files.OrderBy(f => f.Name);
        }

        public bool IsOwner(IPrincipal User)
        {
            return UpdatedBy == User.Identity.Name;
        }

        public FileType GetFileType()
        {
            string ext = System.IO.Path.GetExtension(this.Location).ToLower();
            switch (ext)
            {
                case ".7z":
                case ".bz2":
                case ".bzip":
                case ".cab":
                case ".dmg":
                case ".gz":
                case ".iso":
                case ".rar":
                case ".tar":
                case ".z":
                case ".zip":
                    return FileType.Archive;

                case ".aiff":
                case ".m4a":
                case ".mp2":
                case ".mp3":
                case ".wav":
                    return FileType.Audio;

                case ".as":
                case ".asp":
                case ".aspx":
                case ".bat":
                case ".c":
                case ".cp":
                case ".cpp":
                case ".cs":
                case ".css":
                case ".gradle":
                case ".java":
                case ".js":
                case ".m":
                case ".matlab":
                case ".ml":
                case ".perl":
                case ".php":
                case ".pl":
                case ".ps1":
                case ".py":
                case ".rb":
                case ".rc":
                case ".sh":
                case ".sql":
                case ".swift":
                case ".vb":
                case ".vbs":
                case ".ws":
                case ".xaml":
                case ".xml":
                case ".xsl":
                case ".yml":
                    return FileType.Code;

                case ".doc":
                case ".docx":
                case ".pub":
                case ".html":
                case ".keynote":
                case ".vsd":
                case ".xps":
                case ".wri":
                case ".rtf":
                case ".wps":
                case ".chm":
                case ".word":
                case ".odm":
                case ".wpd":
                case ".book":
                case ".ps2":
                    return FileType.Document;

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
    }
}