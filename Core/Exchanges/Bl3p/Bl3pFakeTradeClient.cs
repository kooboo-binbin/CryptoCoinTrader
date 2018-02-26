using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Remotes;
using CryptoCoinTrader.Core.Exchanges.Bl3p;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Helpers;
using CryptoCoinTrader.Manifest.Trades;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{

    /// <summary>
    /// Minimum order size is 5 euro
    /// </summary>
    public class Bl3pFakeTradeClient : IBl3pTradeClient
    {
        private readonly Random _random = new Random();
        private readonly IMessageService _messageService;
        public string Name => Constants.Name;

        public Bl3pFakeTradeClient(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void GetBlance()
        {

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
