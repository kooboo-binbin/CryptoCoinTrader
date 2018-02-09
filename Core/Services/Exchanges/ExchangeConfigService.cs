using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
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

        public ExchangeConfigService(IBitstampConfig bitstampConfig, IGdaxConfig gdaxConfig)
        {
            _bitstampConfig = bitstampConfig;
            _gdaxConfig = gdaxConfig;
            configs.Add(bitstampConfig.Name, bitstampConfig);
            configs.Add(gdaxConfig.Name, gdaxConfig);
        }

        public void GetConfigs()
        {
            _bitstampConfig.GetConfigInfo();
            _gdaxConfig.GetConfigInfo();
        }

        public MethodResult Validate(string name, string json)
        {
            return configs[name].Valid(json);
        }
    }
}
