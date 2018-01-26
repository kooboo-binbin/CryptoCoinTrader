using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Infos
{
    public class Ticker
    {
        public CurrencyPair CurrencyPair { get; set; }

        public long Id { get; set; }

        public decimal Volume { get; set; }

        public decimal Price { get; set; }

        public DateTime DateTime { get; set; }

        public TradeType TradeType { get; set; }

        public override string ToString()
        {
            return $"{DateTime} {Id} v:{Volume} p:{Price}";
        }
    }
}
