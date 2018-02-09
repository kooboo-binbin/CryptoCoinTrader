using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;

namespace CryptoCoinTrader.Core.Exchanges.Gdax
{
    public class GdaxFakeTradeClient : IGdaxTradeClient
    {
        private readonly Random _random = new Random();
        private IMessageService _messageService;
        public string Name => Constants.Name;

        public GdaxFakeTradeClient(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void GetAccounts()
        {
            throw new NotImplementedException();
        }

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            var rnd = _random.Next(0, 100);
            var status = rnd < 50 ? OrderStatus.Finished : OrderStatus.Open;
            return new MethodResult<OrderStatus>
            {
                Data = status,
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
