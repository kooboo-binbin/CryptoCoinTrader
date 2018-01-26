using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CryptoCoinTrader.Manifest.Enums;


namespace CryptoCoinTrader.Manifest.Infos
{
    public class OrderBook
    {
        public OrderBook()
        {
            Bids = new List<OrderBookItem>();
            Asks = new List<OrderBookItem>();
        }
        public CurrencyPair CurrencyPair { get; set; }

        /// <summary>
        /// Buy
        /// </summary>
        public List<OrderBookItem> Bids { get; set; }

        /// <summary>
        /// Sell
        /// </summary>
        public List<OrderBookItem> Asks { get; set; }

        public override string ToString()
        {
            return ToString(30);
        }

        public string ToString(int amount)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < amount & i < Bids.Count & i < Asks.Count; i++)
            {
                sb.AppendLine($"{i} \t {Bids[i].Volume:f4} \t {Bids[i].Price:f2} \t\t {Asks[i].Volume:f4} \t {Asks[i].Price:f2}");
            }
            return sb.ToString();
        }
    }
}
