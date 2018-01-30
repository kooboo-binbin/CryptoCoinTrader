using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Trades
{
    public class OrderRequest
    {
        public string ClientOrderId { get; set; }
        public OrderType OrderType { get; set; }
        public TradeType TradeType { get; set; }

        /// <summary>
        /// Gdax product_id
        /// </summary>
        public CurrencyPair CurrencyPair { get; set; }

        public decimal Price { get; set; }

        /// <summary>
        /// GDax size
        /// Desired amount of crypto coin to buy or sell
        /// </summary>
        public decimal Volume { get; set; }


        ///Gdax also support the client specify Funds to decide how much funds can be used to buy the coin
    }
}
