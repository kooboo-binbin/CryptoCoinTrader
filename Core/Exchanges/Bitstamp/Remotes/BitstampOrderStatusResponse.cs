using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Remotes
{
    public class BitstampOrderStatusResponse
    {
        public string status { get; set; }
        public Transaction[] transactions { get; set; }
    }

    public class Transaction
    {
        public string fee { get; set; }
        public string price { get; set; }
        public string datetime { get; set; }
        public string usd { get; set; }
        public string btc { get; set; }
        public string tid { get; set; }
        public string type { get; set; }
    }
}
