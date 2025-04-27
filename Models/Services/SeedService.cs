using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Projekt.Data;

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
                //ensure the database is created
                logger.LogInformation("Ensuring the database is created.");
                await context.Database.EnsureCreatedAsync();

                //create roles
                logger.LogInformation("Creating roles.");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "User");

                //create users admin

                logger.LogInformation("Sending admin user.");
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
                        logger.LogInformation("Admin user created.");
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        logger.LogError("Admin user not created.");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError(error.Description);
                        }
                    }
                }
                else
                {
                    logger.LogInformation("Admin user already exists.");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database.");
            }
        }

            private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if(!result.Succeeded)
                    {
                        throw new Exception($"An error occurred creating the {roleName} role: {string.Join(", ", result.Errors.Select(e =>e.Description))}");
                    }
                }
            }
    }
    
}
