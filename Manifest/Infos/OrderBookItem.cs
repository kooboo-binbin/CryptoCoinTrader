using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Infos
{
    public class OrderBookItem
    {
        public string Original { get; set; }

        /// <summary>
        /// For Crypto coin normally
        /// </summary>
        public decimal Volume1 { get; set; }

        /// <summary>
        /// For USD, Eur normally
        /// </summary>
        public decimal Volume2 { get; set; }
    }
}
