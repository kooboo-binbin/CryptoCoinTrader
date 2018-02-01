using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchangeData
    {
        string Name { get; }

        DateTime DateLastUpdated { get; }

        void Register(List<CurrencyPair> paris);

        void Start();

        OrderBook GetOrderBook(CurrencyPair pair);


    }
}
