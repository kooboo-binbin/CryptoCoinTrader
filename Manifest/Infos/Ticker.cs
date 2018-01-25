using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Infos
{
    public class Ticker
    {
       // public string Original { get; set; }

        public long Id { get; set; }

        public decimal Volume { get; set; }

        public decimal Price { get; set; }
        public DateTime DateTime { get; set; }

        public string BuyOrderId { get; set; }

        public string SellerOrderId { get; set; }

    }
}
