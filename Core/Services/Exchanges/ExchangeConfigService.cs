using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Bl3p.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Interfaces;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public class ExchangeConfigService : IExchangeConfigService
    {
        private readonly Dictionary<string, IExchangeConfig> configs = new Dictionary<string, IExchangeConfig>();
        private readonly IBitstampConfig _bitstampConfig;
        private readonly IGdaxConfig _gdaxConfig;
        private readonly IBl3pConfig _bl3PConfig;

        public ExchangeConfigService(IBitstampConfig bitstampConfig, IGdaxConfig gdaxConfig, IBl3pConfig bl3pConfig)
        {
            _bitstampConfig = bitstampConfig;
            _gdaxConfig = gdaxConfig;
            _bl3PConfig = bl3pConfig;
            configs.Add(bitstampConfig.Name, bitstampConfig);
            configs.Add(gdaxConfig.Name, gdaxConfig);
            configs.Add(bl3pConfig.Name, bl3pConfig);
        }

        public void GetConfigs()
        {
            _bitstampConfig.GetConfigInfo();
            _gdaxConfig.GetConfigInfo();
            _bl3PConfig.GetConfigInfo();
        }

        public MethodResult Validate(string name, string json)
        {
            return configs[name].Valid(json);
        }
    }
}
