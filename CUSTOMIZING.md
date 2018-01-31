# Customizing
wwwplatform&#46;net is designed to be extended and customized while using generally accepted standard tools and components.
- [Customizing](#customizing)
    - [Database Schema](#database-schema)
        - [Custom Models](#custom-models)
        - [Auditable Models](#auditable-models)
        - [Permissible Models](#permissible-models)
        - [Migrations](#migrations)
    - [Views Layouts, and Skins](#views-layouts-and-skins)
        - [Custom Skins](#custom-skins)
        - [Custom Layouts](#custom-layouts)

## Database Schema
wwwplatform&#46;net uses [Entity Framework](https://docs.microsoft.com/en-us/ef/) (EF) to abstract the database layer into code.
This is referred to as code-first development.
You can extend the database schema by modifying the ApplicationDbContext class.

### Custom Models 

I recommend adding a new file to the project for your customizations to make merging updates from the original repo easier.
For example: Create a file called `ApplicationDbContext.custom.cs` in src/web/Models with this partial class:

```csharp
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace wwwplatform.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
    
    }
}
```
You can then begin adding your own models and DbSets.

### Auditable Models
The `Auditable` base class is used for basic model behaviors and attributes such as 

- A numeric ID field
- Created and Updated date stamps
- Soft delete behavior
- Created by (user name) field

The soft delete behavior is enforced by the use of this construct:
```csharp
public IQueryable<ModelName> ActiveModelNames { get { return Active<ModelName>(); } }
```
So that instead of querying the database using `db.ModelNames` you would use `db.ActiveModelNames`
You must also map your Auditable type by adding `MapAuditable<ModelName>(modelBuilder);` in `ApplicationDbContext.config.cs`

### Permissible Models
By implementing the `Permissible` interface, your model can have permissions applied by system role, securing content to be accessed by one type of user and not by another.
The API `Permission.GetPermissible<ModelName>(...)` enables the proper filtering of such models based on the user's roles and access.
You must also map your Permissible type by adding this call in `ApplicationDbContext.config.cs`
```csharp
modelBuilder.Entity<ModelName>()
    .HasMany(m => m.Permissions)
    .WithOptional()
    .WillCascadeOnDelete(false);
```

### Migrations
You may have noticed that wwwplatform&#46;net does not have any of its own migrations.
This is so that the developer can control how migrations are applied.
By default the migration method is to auto-create migrations on the fly, meaning whatever changed in ApplicationDbContext will be applied on next startup.
While this makes it easy to do development, it can make database updates unpredictable and difficult to manage.
I recommend disabling auto-migrate and creating your migrations when you first create the project and any time you merge updates from the original repo.

## Views Layouts, and Skins
"Out of the box", wwwplatform&#46;net implements a fairly basic [Bootstrap 3](https://getbootstrap.com/docs/3.3/) based UI.
For simple pages, this will probably be enough.
But adding your own styling is fairly easy.
You can start with adding your own controllers and views and of course editing the existing cshtml files.

### Custom Skins
One way to change the look of the entire site is with the use of skins.
Open up [src/web/App_Data/Skins/Default/skin.json](src/web/App_Data/Skins/Default/skin.json).
You can copy this file and edit it to create your custom skin.
There are three attributes for `scripts`, `css`, and `layout`.
Under `scripts` you will notice an asset group for `"~/scripts/modernizr"` and `"~/scripts/substance"`.
These are rendered and minified by the asset pipeline in Views/Shared/_Layout.cshtml (the default layout).
By including asset groups of scripts and css in your custom skin, you can apply different layouts, behaviors and styling to individual pages or change the way default pages look and function.

Only one skin can be active at a time.
To change the skin that is used for the site, set the "Skin Definition File" value on the settings page.
Changing the "Skin Definition File" value will restart the web app to reload all bundles.

### Custom Layouts
The `layout` attribute in the skin definition file can be set to a custom layout for all pages or left `null` to use the default layout file.
The syntax for the layout file location is `~/Views/Shared/_Layout.cshtml` (the default).
You can also change the default layout page by setting the "Default Page Layout" value on the settings page if you don't want to create an entirely different skin.

The default layout will be used for all of the built-in pages such as login, registration, site page editor, mailing lists, etc. unless you edit those views manually.
For all other pages and views you can set a custom layout individually.
