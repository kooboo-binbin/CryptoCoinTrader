using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Bl3p;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using CryptoCoinTrader.Manifest.Interfaces;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public class ExchangeDataService : IExchangeDataService
    {
        private readonly Dictionary<string, IExchangeData> _dictData = new Dictionary<string, IExchangeData>();
        private readonly List<string> _exchangeNames = new List<string>();

        public ExchangeDataService(IBitstampDataClient bitstamp, IGdaxDataClient gdax, IBl3pDataClient bl3p)
        {
            var exchanges = new List<IExchangeData>() { bitstamp, gdax, bl3p };
            foreach (var exchange in exchanges)
            {
                var name = exchange.Name.ToLower();
                _dictData.Add(name, exchange);
                _exchangeNames.Add(name);
            }
        }

        public List<string> GetExchangeNames()
        {
            return _exchangeNames;
        }

        public OrderBook GetOrderBook(string name, CurrencyPair pair)
        {
            return _dictData[name.ToLower()].GetOrderBook(pair);
        }

        public DateTime GetLastUpdated(string name)
        {
            return _dictData[name.ToLower()].DateLastUpdated;
        }

        public void Register(List<CurrencyPair> paris)
        {
            foreach (var item in _dictData)
            {
                item.Value.Register(paris);
            }
        }

        public void Start()
        {
            foreach (var item in _dictData)
            {
                item.Value.Start();
            }
        }
    }
}
