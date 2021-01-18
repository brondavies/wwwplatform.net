namespace wwwplatform.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using wwwplatform.Models;

    public partial class InstallSqlState : DbMigration
    {
        public override void Up()
        {
            RunAspnetRegSql();
        }

        private void RunAspnetRegSql()
        {
            var connectionstring = new ApplicationDbContext().Database.Connection.ConnectionString;
            var framework = RuntimeEnvironment.GetRuntimeDirectory();
            var filename = Path.Combine(framework, "aspnet_regsql.exe");
            var args = $"-ssadd -sstype c -Q -C \"{connectionstring}\"";
            Process.Start(filename, args).WaitForExit();
        }

        public override void Down()
        {
        }
    }
}
