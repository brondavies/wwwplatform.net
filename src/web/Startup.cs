using wwwplatform.Models;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(wwwplatform.Startup))]
namespace wwwplatform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ApplicationDbContext.Create().Setup();
            ConfigureAuth(app);
        }
    }
}
