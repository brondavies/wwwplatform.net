# Setup
- [Setup](#setup)
    - [Name Your Project](#name-your-project)
    - [Connection String](#connection-string)
    - [First Admin User](#first-admin-user)
- [Deploy](#deploy)
    - [Database Migrations](#database-migrations)
    - [App Settings](#app-settings)

## Name Your Project
At this point you may want to rename the solution file to `yourproject.sln` so you can more easily identify which project it is if you have more than one project based on wwwplatform. You can safely check this in and still merge updates from the base repo.  You should **not** rename the .csproj files as this will make it more difficult to merge in updates from the base repo.

## Connection String
The default database connection string "DefaultConnection" is set in the web&#46;config at `src/web/web.config` to a LocalDB - this is so that you can run the project without any additional setup steps.  However, most developers prefer to use a database server. You should update this connection string now.  Unit tests still use LocalDB connections and re-create the test database with each run.

**Note:** I recommend editing the Web.Debug.config file for local debugging changes or Web.Release.config for production changes so that merging changes to Web.config from the original repo is easier.

## First Admin User
When you run the app for the first time, the database will be initialized with Entity Framework auto-migrations. (more info on migrations below) You will be presented with the default home page. However, there are no default admin users or any other built-in users.  Instead, navigate to http://localhost:53812/Account/Register and create an account. ***The first account you create will become an Administrator automatically.*** Subsequent accounts will be created as members of the Users role.

# Deploy
You can choose any method for deploying your app that you like (File system, FTP, WebDeploy, Azure, etc.) These methods will all behave the same upon first start up.  The default behavior is to use auto-migrations to create the database schema but you must have a connection string pointing to an existing database (preferrably one that is empty). On subsequent updates to your deployed site, you should **not** opt to delete existing files in the target because you may have files uploaded in the UserFiles directory.

**Note:** Remember that Visual Studio will not deploy files in your project directory that are not associated to the project in wwwplatform.net.csproj

## Database Migrations
Every time the app starts up, the database will be checked for consistency with the model and auto-migrations applied. You can disable this behavior by setting the value of `AutoMigrateDatabaseToLatestVersion` in web&#46;config to `False`. If you do, you will have to run the migrations manually or as part of the deployment. For custom build and deployment environments, the utility `migrate.exe` is provided to enable running migrations as part of a script.  Simply call `bin\migrate.exe` from your deployment script and it will read the web&#46;config for the connection string.  You can also optionally pass a connection string to `migrate.exe` *See [Migrate.cs](src/migrate/Migrate.cs)*.

## App Settings
The page at `/AppSettings` allows changes to basic site settings which are stored in the database.  Most of these are self-explanatory but others can actually cause your site to stop loading if not set correctly.  Here's a [quick reference](APPSETTINGS.md).
