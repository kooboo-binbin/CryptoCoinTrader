using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;
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
            services.AddDbContext<CoinContext>(options =>
            {
                options.UseSqlite("Data Source=cointrader.db");
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            var appSettings = new AppSettings();
            Configuration.GetSection("app").Bind(appSettings);
            var context = new FileLoggerContext(AppContext.BaseDirectory, "coin.log");

            services.AddSingleton(new LoggerFactory().AddConsole().AddFile(context, Configuration));
            services.AddLogging();
            services.AddSingleton(Configuration);
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
