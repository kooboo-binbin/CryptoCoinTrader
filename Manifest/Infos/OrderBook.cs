using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Infos
{
    public class OrderBook
    {
        public CurrencyPair CurrencyPair { get; set; }

        /// <summary>
        /// Buy
        /// </summary>
        public List<OrderBookItem> Bids { get; set; }

        /// <summary>
        /// Sell
        /// </summary>
        public List<OrderBookItem> Asks { get; set; }
    }
}
