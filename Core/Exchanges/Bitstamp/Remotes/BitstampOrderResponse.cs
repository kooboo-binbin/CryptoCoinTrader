using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Remotes
{
    public class BitstampOrderResponse
    {
        public string id { get; set; }
        public string datetime { get; set; }
        public string price { get; set; }
        public string amount { get; set; }

        public string status { get; set; }
        public string reason { get; set; }

        public OrderResult Convert()
        {
            var order = new OrderResult() { Id = id, Status = OrderStatus.Open };
            return order;
        }
    }
}
