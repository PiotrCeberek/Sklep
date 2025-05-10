using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Projekt.Data;
using Projekt.Data.Projekt.Services;
using Projekt.Models;
using Projekt.Models.Services;

namespace Projekt
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);    
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<Users, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddTransient<Projekt.Models.Email.EmailService>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            });
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<ApiService>();
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var userManager = serviceProvider.GetRequiredService<UserManager<Users>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = new[] { "Admin", "User", "Employee" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var adminEmail = "admin@example.com";
                var adminPassword = "Admin123!";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new Users
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        NormalizedUserName = adminEmail.ToUpper(),
                        FullName = "Administrator",
                        Role = "Admin"
                    };

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                        Console.WriteLine($"Domy�lny admin utworzony: Email: {adminEmail}, Has�o: {adminPassword}");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"B��d podczas tworzenia admina: {error.Description}");
                        }
                    }
                }

                await SeedService.SeedDatabase(serviceProvider);
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            Console.WriteLine($"Connected to: {builder.Configuration.GetConnectionString("DefaultConnection")}");


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Use(async (context, next) =>
            {
                var waluta = context.Session.GetString("WybranaWaluta");
                if (string.IsNullOrEmpty(waluta))
                {
                    context.Session.SetString("WybranaWaluta", "PLN");
                }
                var mnoznik = context.Session.GetString("Mnoznik");
                if (string.IsNullOrEmpty(waluta))
                {
                    context.Session.SetString("Mnoznik", "1");
                }

                await next();
            });

            app.Run();
        }
    }
}