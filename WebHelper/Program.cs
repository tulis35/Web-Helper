using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHelper.Data;

namespace WebHelper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var h = CreateHostBuilder(args).Build();
            
            using(var scope = h.Services.CreateScope())
            {
                var fileManager = scope.ServiceProvider.GetRequiredService<IFileManager>();

                try
                {
                    // Zavoláme vaši asynchronní metodu pro vytvoření složek
                    await fileManager.CreateFolders();
                }
                catch (Exception ex)
                {
                    // Doporučuji odchytit případnou chybu (např. chybějící práva k zápisu na disk)
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Došlo k chybě při vytváření složek při startu aplikace.");
                }
            }
            await h.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IFileManager, FileManager>();
        }
    }
}
