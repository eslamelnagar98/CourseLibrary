using CourseLibrary.API.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace CourseLibrary.API
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();
            Log.Logger = new LoggerConfiguration()
                           .ReadFrom.Configuration(config)
                           //.WriteTo.File(path: @"Serilog\\log.json", rollingInterval: RollingInterval.Day)
                           .CreateLogger();
            Log.Information("App Starting");
            try
            {
                var host = CreateHostBuilder(args).Build();
                MigrateDatabase(host);
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("App Crashed Due to {@exception}", ex);

            }

        }

        private static void MigrateDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            try
            {
                CourseLibraryContext context = scope.ServiceProvider.GetService<CourseLibraryContext>();
                //context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError("An error occurred while migrating the database. {@ex}", ex);
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
    }
}
