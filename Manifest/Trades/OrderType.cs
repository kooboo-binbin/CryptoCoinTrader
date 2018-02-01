using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Trades
{
    /// <summary>
    /// Limit, Market, Stop 
    /// Exchange Bitstamp does not have stop
    /// </summary>
    public enum OrderType
    {
        Limit = 0,
        Market = 1,
        Stop = 2,
    }
}
