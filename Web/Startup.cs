using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Exchanges;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Arbitrages;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Core.Services.Messages;
using CryptoCoinTrader.Core.Services.Observations;
using CryptoCoinTrader.Core.Services.Orders;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Manifest.Interfaces;
using CryptoCoinTrader.Web;
using Karambolo.Extensions.Logging.File;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CryptoCoinTrader
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings();
            Configuration.GetSection("app").Bind(appSettings);
            var context = new FileLoggerContext(AppContext.BaseDirectory, "coin.log");

            services.AddSingleton(new LoggerFactory().AddFile(context, Configuration));
            services.AddLogging();
            services.AddSingleton(Configuration);
            services.AddSingleton(appSettings);

            services.AddDbContext<CoinContext>(options =>
            {
                options.UseSqlite("Data Source=cointrader.db");
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }, ServiceLifetime.Scoped);

            services.AddMemoryCache();
            services.AddScoped<ISelfInspectionService, SelfInspectionService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IBitstampConfig, BitstampConfig>();
            services.AddScoped<IGdaxConfig, GdaxConfig>();
            services.AddScoped<IBitstampCurrencyMapper, BitstampCurrencyMapper>();
            services.AddScoped<IGdaxCurrencyMapper, GdaxCurrencyMapper>();
            services.AddScoped<IGdaxOrderStatusMapper, GdaxOrderStatusMapper>();
            services.AddScoped<IBitmapOrderStatusMapper, BitstampOrderStatusMapper>();
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
            services.AddScoped<IObservationService, ObservationService>();
            services.AddScoped<IExchangeDataService, ExchangeDataService>();
            services.AddScoped<IExchangeTradeService, ExchangeTradeService>();
            services.AddScoped<IMessageService, ConsoleMessageService>();
            services.AddScoped<IOpportunityService, OpportunityService>();
            services.AddScoped<IArbitrageService, ArbitrageService>();
            services.AddScoped<IExchangeSetting, ExchangeSetting>();
            services.AddScoped<IExchangeConfigService, ExchangeConfigService>();
            services.AddScoped<IWorker, Worker>();
            services.AddScoped<App, App>();
            services.AddOptions();

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                try
                {
                    app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                    {
                        HotModuleReplacement = true
                    });
                }
                catch (Exception ex)
                {
                    //the image does not installed node js
                }
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
