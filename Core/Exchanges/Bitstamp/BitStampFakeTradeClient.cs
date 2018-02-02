using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{
    public class BitstampFakeTradeClient : IBitstampTradeClient
    {
        public string Name => "bitstamp";

        public void GetBlance()
        {

        }

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            return new MethodResult<OrderStatus>
            {
                Data = OrderStatus.Finished,
                IsSuccessful = true
            };
        }

        public MethodResult<OrderResult> MakeANewOrder(OrderRequest order)
        {
            return new MethodResult<OrderResult>
            {
                Data = new OrderResult
                {
                    Id = order.ClientOrderId,
                    Status = OrderStatus.Open
                },
                IsSuccessful = true
            };
        }
    }
}
