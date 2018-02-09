using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using CryptoCoinTrader.Core.Exchanges;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using RestSharp;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using System.IO;
using CryptoCoinTrader.Core;
using Microsoft.Extensions.Logging;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;

using Karambolo.Extensions.Logging.File;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Core.Data;
using Microsoft.EntityFrameworkCore;
using CryptoCoinTrader.Core.Services.Orders;
using CryptoCoinTrader.Core.Services.Messages;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Core.Services.Arbitrages;

namespace TradeConsole
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            SetCulture();
            _serviceProvider = BuilderDi();
            _serviceProvider.GetService<CoinContext>().Database.EnsureCreated();
            _serviceProvider.GetService<App>().Run();
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

            services.AddSingleton(new LoggerFactory().AddFile(context, configuration));
            services.AddLogging();
            services.AddSingleton(configuration);
            services.AddSingleton(appSettings);

            services.AddDbContext<CoinContext>(options =>
            {
                options.UseSqlite("Data Source=cointrader.db");
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Transient);

            services.AddMemoryCache();
            services.AddSingleton<ISelfInspectionService, SelfInspectionService>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<IBitstampConfig, BitstampConfig>();
            services.AddSingleton<IGdaxConfig, GdaxConfig>();
            services.AddSingleton<IBitstampCurrencyMapper, BitstampCurrencyMapper>();
            services.AddSingleton<IGdaxCurrencyMapper, GdaxCurrencyMapper>();
            services.AddSingleton<IGdaxOrderStatusMapper, GdaxOrderStatusMapper>();
            services.AddSingleton<IBitmapOrderStatusMapper, BitstampOrderStatusMapper>();
            services.AddSingleton<IBitstampDataClient, BitstampDataClient>();
            services.AddSingleton<IGdaxDataClient, GdaxDataClient>();
            if (appSettings.Production)
            {
                services.AddSingleton<IBitstampTradeClient, BitstampTradeClient>();
                services.AddSingleton<IGdaxTradeClient, GdaxTradeClient>();
            }
            else
            {
                services.AddSingleton<IBitstampTradeClient, BitstampFakeTradeClient>();
                services.AddSingleton<IGdaxTradeClient, GdaxFakeTradeClient>();
            }
            services.AddSingleton<IObservationService, ObservationFileService>();
            services.AddSingleton<IExchangeDataService, ExchangeDataService>();
            services.AddSingleton<IExchangeTradeService, ExchangeTradeService>();
            services.AddSingleton<IMessageService, ConsoleMessageService>();
            services.AddSingleton<IOpportunityService, OpportunityService>();
            services.AddSingleton<IArbitrageService, ArbitrageService>();
            services.AddSingleton<IWorker, Worker>();

            services.AddOptions();
            services.AddTransient<App>();

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

    }
}
