using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{
    public class BitstampFakeTradeClient : IBitstampTradeClient
    {
        private readonly Random _random = new Random();
        private readonly IMessageService _messageService;
        public string Name => "bitstamp";

        public BitstampFakeTradeClient(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void GetBlance()
        {

        }

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            _messageService.Write(20, $"bitstamp \t {orderId}");
            var rnd = _random.Next(0, 100);
            var status = rnd < 20 ? OrderStatus.Finished : OrderStatus.Open;
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
