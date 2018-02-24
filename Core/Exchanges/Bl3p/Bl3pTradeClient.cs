using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{
    public class Bl3pTradeClient : IBl3pTradeClient
    {
        public string Name => Constants.Name;

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            return new MethodResult<OrderStatus>() { IsSuccessful = false };
        }

        public MethodResult<OrderResult> MakeANewOrder(OrderRequest order)
        {
            return new MethodResult<OrderResult>()
            {
                IsSuccessful = false,
                Data = new OrderResult() { }
            };
        }
    }
}
