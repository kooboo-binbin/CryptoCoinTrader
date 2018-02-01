using CryptoCoinTrader.Manifest.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchangeTrade
    {
        string Name { get; }
        MethodResult<OrderResult> MakeANewOrder(OrderRequest order);
    }
}
