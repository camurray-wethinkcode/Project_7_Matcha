using System;
using Matcha.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Matcha.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    if (!System.IO.File.Exists("matcha.db"))
                    {
                        Console.WriteLine("Creating Database...");
                        var dbAccess = services.GetRequiredService<IDbAccess>();
                        var createTablesCommand = System.IO.File.ReadAllText("CreateTables.sql");
                        var rowsAffected = dbAccess.NonQuery(createTablesCommand).Result;
                        Console.WriteLine("{0} rows affected during database creation.", rowsAffected);
                    }

                    var userDataContext = services.GetRequiredService<IUserDataContext>();
                    Seed.SeedUsers(userDataContext);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred during DB setup and migration");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
