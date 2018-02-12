using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public interface IExchangeDataService
    {
        List<string> GetExchangeNames();
        void Register(List<CurrencyPair> paris);
        void Start();
        OrderBook GetOrderBook(string name, CurrencyPair pair);
        DateTime GetLastUpdated(string name);
    }
}
