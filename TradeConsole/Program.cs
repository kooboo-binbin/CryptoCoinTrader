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
using Microsoft.Extensions.Options;
using Karambolo.Extensions.Logging.File;
using CryptoCoinTrader.Core.Services.Exchanges;

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

            var services = new ServiceCollection();

            var context = new FileLoggerContext(AppContext.BaseDirectory, "coin.log");

            services.AddSingleton(new LoggerFactory().AddConsole().AddFile(context, configuration));
            services.AddLogging();
            services.AddSingleton(configuration);
            services.AddSingleton(appSettings);
            services.AddSingleton<ISelfInspectionService, SelfInspectionService>();
            services.AddSingleton<IBitstampConfig, BitstampConfigFile>();
            services.AddSingleton<IGdaxConfig, GdaxConfigFile>();
            services.AddSingleton<IBitstampCurrencyMapper, BitstampCurrencyMapper>();
            services.AddSingleton<IGdaxCurrencyMapper, GdaxCurrencyMapper>();
            services.AddSingleton<IBitstampDataClient, BitstampDataClient>();
            services.AddSingleton<IGdaxDataClient, GdaxDataClient>();
            services.AddSingleton<IBitstampTradeClient, BitstampTradeClient>();
            services.AddSingleton<IGdaxTradeClient, GdaxTradeClient>();
            services.AddSingleton<IObservationService, ObservationFileService>();
            services.AddSingleton<IExchangeDataService, ExchangeDataService>();
            services.AddSingleton<IExchangeTradeService, ExchangeTradeService>();

            services.AddOptions();
            services.AddTransient<App>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }


    }
}
