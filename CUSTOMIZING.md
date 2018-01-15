# Customizing
wwwplatform&#46;net is designed to be extended and customized while using generally accepted standard tools and components.

## Database Schema
wwwplatform&#46;net uses [Entity Framework](https://docs.microsoft.com/en-us/ef/) (EF) to abstract the database layer into code.
This is referred to as code-first development.  You can extend the database schema by modifying the ApplicationDbContext class.

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