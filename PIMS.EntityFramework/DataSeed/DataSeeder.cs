using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PIMS.Core.Models;
using PIMS.EntityFramework.EntityFramework;

namespace PIMS.EntityFramework.DataSeed
{
    public class DataSeeder
    {
        public static async Task SeedData(PrimsDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            await SeedRoles(roleManager);

            await SeedUsers(userManager);

            await SeedCategories(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole<int>("User"));
            }
        }

        private static async Task SeedUsers(UserManager<User> userManager)
        {
            if (await userManager.FindByEmailAsync("admin@pims.com") == null)
            {
                var adminUser = new User
                {
                    UserName = "admin@pims.com",
                    Email = "admin@pims.com",
                    FullName = "Admin User",
                    Role = "Admin"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (await userManager.FindByEmailAsync("user@pims.com") == null)
            {
                var regularUser = new User
                {
                    UserName = "user@pims.com",
                    Email = "user@pims.com",
                    FullName = "Regular User",
                    Role = "User"
                };

                var result = await userManager.CreateAsync(regularUser, "User@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }
        }

        private static async Task SeedCategories(PrimsDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category { CategoryName = "Electronics" },
                    new Category { CategoryName = "Clothing" },
                    new Category { CategoryName = "Groceries" }
                };

                await context.Categories.AddRangeAsync(categories);
            }
        }
    }
}
