using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using User.Management.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Data;
using User.Management.API.Enum;

namespace User.Management.API.Extension
{
    public static class ServicesExtension
    {

        public async static void AddDatabaseSeed(WebApplication builder)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
               .MinimumLevel.Override("System", LogEventLevel.Warning)
               .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
               .CreateLogger();
            try
            {

                // If there is no scope create it for database migration
                using (var scope = builder.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    // Get context service 
                    var applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

                    if (!applicationDbContext.Database.GetAppliedMigrations().SequenceEqual(applicationDbContext.Database.GetMigrations()))
                    {
                        applicationDbContext.Database.MigrateAsync().Wait();
                    }

                    // If there is no user create it
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    if (!userManager.Users.Any())
                    {
                        var user = new ApplicationUser
                        {
                            FirstName = "Muhammed",
                            LastName = "Aldemir",
                            UserName = "muhammedaldemir",
                            PhoneNumber = "+905382051031",
                            Email = "aldemirrmuhammed2009@gmail.com",
                            City = "Ankara",
                        };

                        // Create default user
                        userManager.CreateAsync(user, "Password12*").Wait();
                        var roleList = await userManager.GetRolesAsync(user);
                        foreach (var role in roleList)
                        {
                            if (roleManager.RoleExistsAsync(role.ToString()).Result)
                            {
                                if (!(await userManager.IsInRoleAsync(user, role.ToString())))
                                {
                                    await userManager.AddToRoleAsync(user, role.ToString());
                                }
                            }
                        }
                    }
                }
                Log.Information("Starting host...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

}
