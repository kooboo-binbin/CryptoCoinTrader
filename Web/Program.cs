using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using CryptoCoinTrader.Core.Data;
using System.Globalization;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Web;

namespace CryptoCoinTrader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-us");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-us");

            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CoinContext>();
                context.Database.EnsureCreated();

                var app = services.GetRequiredService<App>();
                app.Run();
            }

            host.Run();
        }


        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
            .UseUrls("http://*:5000")
               .Build();
    }
}
