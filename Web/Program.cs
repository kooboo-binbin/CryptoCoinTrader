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

namespace CryptoCoinTrader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-us");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-us");

            MethodResult inspectionResult;
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<CoinContext>();
                context.Database.EnsureCreated();
                var inspectionService = services.GetService<ISelfInspectionService>();
                inspectionResult = inspectionService.Inspect();
                RunObservatoin(inspectionResult, services);
            }
#if Debug
            if (inspectionResult.IsSuccessful)
            {
                host.Run();
            }
#else
            host.Run();
#endif
        }
        private static void RunObservatoin(MethodResult inspectionResult, IServiceProvider services)
        {
            if (!inspectionResult.IsSuccessful)
            {
                Console.WriteLine(inspectionResult.Message);
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
            }
            else
            {
                Task.Run(() =>
                {
                    Console.Write("Start");
                    var observatoinServices = services.GetRequiredService<IObservationService>();
                    var worker = services.GetRequiredService<IWorker>();
                    worker.Work(observatoinServices.GetObservations());
                });
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
           WebHost.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
            .UseUrls("http://*:5000")
               .Build();
    }
}
