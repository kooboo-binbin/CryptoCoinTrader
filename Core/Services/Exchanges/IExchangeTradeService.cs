using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Trades;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public interface IExchangeTradeService
    {
        /// <summary>
        /// Make a new order
        /// </summary>
        /// <param name="name">Exchange name</param>
        /// <param name="order"></param>
        /// <returns></returns>
        MethodResult<OrderResult> MakeANewOrder(string name, OrderRequest order);
    }
}
