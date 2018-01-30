using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Trades
{
    public class OrderResult
    {
        public string Id { get; set; }

        public OrderStatus Status { get; set; }
    }
}
