using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Manifest.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CryptoCoinTrader.Core.Exchanges
{
    public class ExchangeSetting : IExchangeSetting
    {
        private readonly CoinContext _coinContext;
        public ExchangeSetting(CoinContext coinContext)
        {
            _coinContext = coinContext;
        }

        public string GetSetting(string name)
        {
            var exchange = _coinContext.Exchanges.FirstOrDefault(it => it.Name.ToLower() == name.ToLower());
            return exchange?.Settings;
        }

        public void SaveSetting(string name, string settings)
        {
            name = name.ToLower();
            var exchange = _coinContext.Exchanges.FirstOrDefault(it => it.Name == name);
            if (exchange == null)
            {
                exchange = new Data.Entities.Exchange();
                exchange.Name = name;
                exchange.Settings = settings;
                _coinContext.Add(exchange);
            }
            else
            {
                exchange.Settings = settings;
            }
            _coinContext.SaveChanges();

        }
    }
}
