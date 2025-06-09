using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Projekt.Data;
using System;

namespace Projekt.Models.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();

            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                await context.Database.EnsureCreatedAsync();
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");
                var adminEmial = "admin@wp.pl";
                if (await userManager.FindByEmailAsync(adminEmial) == null)
                {
                    var admin = new Users
                    {
                        FullName = "Admin",
                        UserName = adminEmial,
                        Email = adminEmial,
                        NormalizedEmail = adminEmial.ToUpper(),
                        NormalizedUserName = adminEmial.ToUpper(),
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = "Admin"

                    };
                    var result = await userManager.CreateAsync(admin, "Admin@123!");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Stworzono konto administratora!.");
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        logger.LogError("Błąd podczas tworzenia konta administratora.");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError(error.Description);
                        }
                    }
                }
                else
                {
                    logger.LogInformation("Administrator już istnieje.");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Wystąpił błąd podczas inicjalizacji bazy danych.");
            }
        }

            private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
    }
    
}
