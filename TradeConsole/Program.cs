using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CryptoCoinTrader.Core.Exchanges;
using CryptoCoinTrader.Core.Exchanges.BitStamp;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using RestSharp;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.IO;
using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TradeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SetCulture();
            var serviceProvider = BuilderDi();
            serviceProvider.GetService<App>().Run();
        }

        private static void SetCulture()
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-us");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-us");
        }


        private static IServiceProvider BuilderDi()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var appSettings = new AppSettings();
            configuration.GetSection("app").Bind(appSettings);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(new LoggerFactory().AddConsole());
            //serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton(appSettings);
            serviceCollection.AddOptions();
            serviceCollection.AddTransient<App>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            //var logFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            //logFactory.AddConsole();
            return serviceProvider;
        }

       
    }
}
