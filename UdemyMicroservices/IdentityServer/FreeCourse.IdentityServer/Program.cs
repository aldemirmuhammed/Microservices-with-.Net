// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using FreeCourse.IdentityServer.Data;
using FreeCourse.IdentityServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
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
                var host = CreateHostBuilder(args).Build();

                // If there is no scope create it for database migration
                using (var scope = host.Services.CreateScope())
                {
                    var serviceProvider = scope.ServiceProvider;

                    // Get context service 
                    var applicationDbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

                    // If there is no database or not applied migration,this create database or apply migration folder when run application
                    applicationDbContext.Database.Migrate();

                    // If there is no user create it  
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    if (!userManager.Users.Any())
                    {
                        // Create default user
                        userManager.CreateAsync(new ApplicationUser
                        {
                            UserName = "muhammedaldemir",
                            PhoneNumber = "+905382051031",
                            Email = "aldemirrmuhammed2009@gmail.com",
                            City = "Ankara"                            
                        }, "Password12*").Wait();
                    }
                    //await SeedData.EnsureSeedData(host.Configuration.GetConnectionString("DefaultConnection"));
                }
                Log.Information("Starting host...");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}