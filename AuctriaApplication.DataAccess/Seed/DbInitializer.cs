using System.Security.Claims;
using AuctriaApplication.DataAccess.DbContext;
using AuctriaApplication.DataAccess.Entities;
using AuctriaApplication.DataAccess.Entities.Users;
using AuctriaApplication.Domain.Enums;
using AuctriaApplication.Domain.Variables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuctriaApplication.DataAccess.Seed;

public static class DbInitializer
{
    public static async Task Initializer(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        // Initialize roles first, if not already done
        await SeedRoles(roleManager);

        // Then seed users
        await SeedUsers(userManager);
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
                Name = "AgentManager",
                UserName = "AgentManager",
                Email = "AgentManager@test.com"
            },
            new User
            {
                Name = "Agent",
                UserName = "Agent",
                Email = "agent@test.com"
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
}