using System.Security.Claims;
using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities.Stores;
using AuctriaApplication.DataAccess.Entities.Users;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Domain.Variables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuctriaApplication.DataAccess.Seed;

public static class DbInitializer
{
    private static readonly string _superAdminEmail = "alishahbazi799@gmail.com";
    public static async Task Initializer(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        // Initialize roles first, if not already done
        await SeedRoles(roleManager);

        // Then seed users
        await SeedUsers(userManager);
        
        // Seed categories
        await SeedCategories(context);
        
        // Seed products
        await SeedProducts(context);
    }

    private static async Task SeedRoles(RoleManager<Role> roleManager)
    {
        var roles = new List<string>
            { SharedRolesVar.SuperAdmin, SharedRolesVar.Admin, SharedRolesVar.Member };

        foreach (var roleName in roles)
        {
            if (await roleManager.RoleExistsAsync(roleName))
                continue;
            var role = new Role { Name = roleName, NormalizedName = roleName.ToUpper() };
            await roleManager.CreateAsync(role);

            // Seeding default RoleClaims
            await SeedRoleClaims(roleManager, role, roleName);
        }
    }

    private static async Task SeedRoleClaims(RoleManager<Role> roleManager, Role role, string roleName)
    {
        // Skip processing for SuperAdmin and Member
        if (roleName is SharedRolesVar.SuperAdmin or SharedRolesVar.Member)
            return;

        var permissions = GetPermissionsForRole(roleName);
        foreach (var permission in permissions)
        {
            var claim = new IdentityRoleClaim<string>
            {
                RoleId = role.Id.ToString(),
                ClaimType = "Permission",
                ClaimValue = permission.ToString()
            };
            await roleManager.AddClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
        }
    }

    private static IEnumerable<PermissionAction> GetPermissionsForRole(string roleName)
    {
        return roleName switch
        {
            SharedRolesVar.Admin => new List<PermissionAction>
            {
                PermissionAction.Members_List,
                PermissionAction.Members_Lockout,
            },
            _ => new List<PermissionAction>()
        };
    }

    private static async Task SeedUsers(UserManager<User> userManager)
    {
        if (await userManager.Users.AnyAsync())
            return;

        var users = new List<User>
        {
            new User
            {
                Name = "Admin",
                UserName = "Admin",
                Email = "admin@test.com"
            },
            new User
            {
                Name = "Member",
                UserName = "Member",
                Email = "Member@test.com"
            },
            new User
            {
                Name = "Ali",
                Surname = "Shahbazi",
                UserName = "VorTex",
                Email = "alishahbazi799@gmail.com",
                IsDeleted = false,
                LockoutEnabled = false,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            }
        };

        foreach (var user in users)
        {
            var password = user.Email == "alishahbazi799@gmail.com" ? "Pa$$w0rd" : "123456";
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded) continue;

            string roleName;
            if (user.Email == "alishahbazi799@gmail.com")
            {
                roleName = SharedRolesVar.SuperAdmin;
            }
            else
            {
                switch (user.UserName)
                {
                    case SharedRolesVar.Admin:
                        roleName = SharedRolesVar.Admin;
                        break;
                    case SharedRolesVar.Member:
                        roleName = SharedRolesVar.Member;
                        break;
                    default:
                        continue;
                }
            }

            await userManager.AddToRoleAsync(user, roleName);
        }
    }

    private static async Task SeedCategories(ApplicationDbContext dbContext)
    {
        // Seed categories
        var categories = new List<Category>
        {
            new Category
            {
                Name = "Electronics",
                Description = "Electronics",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Category
            {
                Name = "Clothing",
                Description = "Clothing",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Category
            {
                Name = "Home",
                Description = "Home",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Category
            {
                Name = "Sports",
                Description = "Sports",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Category
            {
                Name = "Toys",
                Description = "Toys",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Category
            {
                Name = "Books",
                Description = "Books",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Category
            {
                Name = "Other",
                Description = "Other",
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            }
        };
        
        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedProducts(ApplicationDbContext dbContext)
    {
        // Seed products
        var products = new List<Product>
        {
            new()
            {
                Name = "Apple iPhone 12 Pro Max",
                Description = "Apple iPhone 12 Pro Max",
                Price = 1099.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 30
            },
            new()
            {
                Name = "Apple iPhone 12 Pro",
                Description = "Apple iPhone 12 Pro",
                Price = 999.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 35
            },
            new()
            {
                Name = "Apple iPhone 12",
                Description = "Apple iPhone 12",
                Price = 799.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 40
            },
            new()
            {
                Name = "Apple iPhone 11 Pro Max",
                Description = "Apple iPhone 11 Pro Max",
                Price = 899.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 45
            },
            new()
            {
                Name = "Apple iPhone 11 Pro",
                Description = "Apple iPhone 11 Pro",
                Price = 799.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 50
            },
            new()
            {
                Name = "Apple iPhone 11",
                Description = "Apple iPhone 11",
                Price = 699.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 55
            },
            new()
            {
                Name = "Apple iPhone X",
                Description = "Apple iPhone X",
                Price = 599.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 60
            },
            new()
            {
                Name = "Apple iPhone 8",
                Description = "Apple iPhone 8",
                Price = 499.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 65
            },
            new()
            {
                Name = "Apple iPhone 7",
                Description = "Apple iPhone 7",
                Price = 399.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 70
            },
            new()
            {
                Name = "Apple iPhone 6",
                Description = "Apple iPhone 6",
                Price = 299.99m,
                Quantity = 34,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Apple iPhone 5",
                Description = "Apple iPhone 5",
                Price = 199.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 75
            },
            new()
            {
                Name = "Apple iPhone 4",
                Description = "Apple iPhone 4",
                Price = 99.99m,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Electronics"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty,
                Quantity = 90
            },
            // Clothing
            new()
            {
                Name = "Nike Air Max 270",
                Description = "Nike Air Max 270",
                Price = 149.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Clothing"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Nike Air Max 720",
                Description = "Nike Air Max 720",
                Price = 199.99m,
                Quantity = 35,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Clothing"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Nike Air Max 90",
                Description = "Nike Air Max 90",
                Price = 99.99m,
                Quantity = 40,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Clothing"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Nike Air Max 95",
                Description = "Nike Air Max 95",
                Price = 129.99m,
                Quantity = 45,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Clothing"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Nike Air Max 97",
                Description = "Nike Air Max 97",
                Price = 149.99m,
                Quantity = 50,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Clothing"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            // Home
            new()
            {
                Name = "Dyson V11",
                Description = "Dyson V11",
                Price = 599.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Home"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Dyson V10",
                Description = "Dyson V10",
                Price = 499.99m,
                Quantity = 35,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Home"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            // Toys
            new()
            {
                Name = "Lego Star Wars",
                Description = "Lego Star Wars",
                Price = 99.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Toys"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            // Books
            new Product
            {
                Name = "Harry Potter",
                Description = "Harry Potter",
                Price = 19.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Books"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new Product
            {
                Name = "Harry Potter 2",
                Description = "Harry Potter 2",
                Price = 19.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Books"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Harry Potter 3",
                Description = "Harry Potter 3",
                Price = 19.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Books"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Harry Potter 4",
                Description = "Harry Potter 4",
                Price = 19.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Books"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Harry Potter 5",
                Description = "Harry Potter 5",
                Price = 19.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Books"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            },
            new()
            {
                Name = "Harry Potter 6",
                Description = "Harry Potter 6",
                Price = 19.99m,
                Quantity = 30,
                CategoryId = (await dbContext.Categories.FirstOrDefaultAsync(x => x.Name == "Books"))?.Id ?? Guid.Empty,
                AddedBy = (await dbContext.Users.FirstOrDefaultAsync(x => x.Email == _superAdminEmail))?.Id ?? Guid.Empty
            }
        };
        
        await dbContext.Products.AddRangeAsync(products);
        await dbContext.SaveChangesAsync();
    }
}