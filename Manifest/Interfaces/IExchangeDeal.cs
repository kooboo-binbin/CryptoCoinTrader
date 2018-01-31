using CryptoCoinTrader.Manifest.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchangeDeal
    {
        MethodResult<OrderResult> MakeANewOrder(OrderRequest order);
    }
}
