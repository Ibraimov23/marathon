using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Project.Data;
using Project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Project
{
    public class AppSeeds
    {
        public static async Task CreateDataAsync(IServiceProvider service)
        {
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = service.GetRequiredService<UserManager<User>>();

            bool existAdminRole = await roleManager.RoleExistsAsync("admin");

            if (!existAdminRole)
            {
                var adminRole = new IdentityRole();
                adminRole.Name = "admin";
                await roleManager.CreateAsync(adminRole);
            }

            bool existCuratorRole = await roleManager.RoleExistsAsync("curator");

            if (!existCuratorRole)
            {
                var curatorRole = new IdentityRole();
                curatorRole.Name = "curator";
                await roleManager.CreateAsync(curatorRole);
            }

            if (!userManager.Users.Any())
            {
                User user = new User { UserName = "555", Email = "555", Phone = 555, Surname = "null", Name = "AdminUser", Patronymic = "null" };
                var result = await userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "admin");
                }
            }

        }

    }
}
