using CryptoCoinTrader.Manifest.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{
    public interface IBitstampTradeClient : IExchangeTrade
    {
        void GetBlance();
    }
}
