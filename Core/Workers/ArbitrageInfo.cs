using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Workers
{
    public class ArbitrageInfo
    {
        public decimal SpreadValue { get; set; }
        public decimal SpreadVolume { get; set; }
        public decimal FromPrice { get; set; }
        public decimal ToPrice { get; set; }
    }
}
