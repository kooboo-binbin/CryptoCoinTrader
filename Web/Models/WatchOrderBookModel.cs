using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Models
{
    public class WatchOrderBookModel
    {
        public string Ask1
        { get; set; }
        public string Ask2
        { get; set; }
        public string Ask3
        { get; set; }
        public string Bid1 { get; set; }

        public string Bid2 { get; set; }

        public string Bid3 { get; set; }

        public string SpreadVolume { get; set; }
        public string SpreadValue { get; set; }
    }
}
