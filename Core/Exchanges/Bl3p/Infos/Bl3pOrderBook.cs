using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p.Infos
{
    public class Bl3pOrderBook
    {
        public string MarketPlace { get; set; }
        public List<Bl3pOrderBookItem> Asks { get; set; }
        public List<Bl3pOrderBookItem> Bids { get; set; }
    }

   
}
