using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
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

        /// <summary>
        /// Get order status by remote orderId
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        MethodResult<OrderStatus> GetOrderStatus(string name, string orderId);
    }
}
