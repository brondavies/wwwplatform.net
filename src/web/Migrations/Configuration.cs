
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using wwwplatform.Models;

namespace wwwplatform.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "wwwplatform.Models.ApplicationDbContext";
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.
            if (0 == context.SitePages.Where(p => p.HomePage).Count())
            {
                context.SitePages.Add(new SitePage
                {
                    Description = "Default Home Page",
                    HomePage = true,
                    HTMLBody = "<h1>Home Page</h1><p>This is the default home page.  You can edit it or set a new home page but you cannot delete it.</p>",
                    Name = "Home Page",
                    PubDate = DateTime.UtcNow
                });
            }
        }

        public void Uninstall(ApplicationDbContext context)
        {
            string dropFK = @"SELECT distinct
                'IF OBJECT_ID(''['+f.name+']'') IS NOT NULL ALTER TABLE [dbo].['+OBJECT_NAME(f.parent_object_id)+'] DROP CONSTRAINT ['+f.name+']' AS drop_sql
                FROM sys.foreign_keys AS f 
                INNER JOIN sys.foreign_key_columns AS fc 
                ON f.OBJECT_ID = fc.constraint_object_id";

            List<string> results = context.Database.SqlQuery<string>(dropFK).ToList();
            string sql = String.Join(";", results);
            if (sql != "") context.Database.ExecuteSqlCommand(sql);

            string dropTables = @"select 'DROP TABLE [' + sys.schemas.name + '].[' + sys.tables.name + ']' as drop_sql from sys.tables
                join sys.schemas on sys.tables.schema_id = sys.schemas.schema_id WHERE sys.tables.type = 'U'";

            results = context.Database.SqlQuery<string>(dropTables).ToList();
            sql = String.Join(";", results);
            if (sql != "") context.Database.ExecuteSqlCommand(sql);

            string dropSP = @"select 'DROP PROC [' + sys.schemas.name + '].[' + sys.procedures.name + ']' as drop_sql from sys.procedures
                join sys.schemas on sys.procedures.schema_id = sys.schemas.schema_id WHERE sys.procedures.type = 'P'";

            results = context.Database.SqlQuery<string>(dropSP).ToList();
            sql = String.Join(";", results);
            if (sql != "") context.Database.ExecuteSqlCommand(sql);

        }
    }
}
