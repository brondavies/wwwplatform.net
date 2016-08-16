using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using wwwplatform.Extensions;
using wwwplatform.Models;

namespace wwwplatform.Controllers
{
    [Extensions.Authorize]
    public class SharedFoldersController : BaseController
    {
        private const string AllowedFields = "Id,Name,Description";

        public async Task<ActionResult> Index()
        {
            var folders = await SharedFolder.GetAvailableFolders(db, User, UserManager, RoleManager).ToListAsync();
            return View(folders);
        }
    }
}