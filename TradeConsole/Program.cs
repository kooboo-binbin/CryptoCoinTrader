using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CryptoCoinTrader.Core.Exchanges;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using RestSharp;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using System.IO;
using CryptoCoinTrader.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;

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
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton(appSettings);
            serviceCollection.AddSingleton<ISelfInspectionService, SelfInspectionService>();
            serviceCollection.AddSingleton<IBitstampConfig, BitstampConfigFile>();
            serviceCollection.AddSingleton<IGdaxConfig, GdaxConfigFile>();
            serviceCollection.AddSingleton<IBitstampTradeClient, BitstampTradeClient>();
            serviceCollection.AddSingleton<IGdaxTradeClient, GdaxTradeClient>();

            serviceCollection.AddOptions();
            serviceCollection.AddTransient<App>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

       
    }
}
