using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Remotes;
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

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{

    /// <summary>
    /// Minimum order size is 5 euro
    /// </summary>
    public class BitstampTradeClient : IBitstampTradeClient
    {
        private readonly string _baseUrl = "https://www.bitstamp.net/api";
        private readonly string _baseUrlV2 = "https://www.bitstamp.net/api/v2";
        private readonly BitstampConfigInfo _config;
        private readonly IBitstampCurrencyMapper _currencyMapper;
        private readonly IBitmapOrderStatusMapper _orderStatusMapper;
        private ILogger<BitstampTradeClient> _logger;
        private readonly CultureInfo _enCulture = new CultureInfo("en-us");


        public BitstampTradeClient(IBitstampConfig config,
            IBitstampCurrencyMapper currencyMapper,
            ILogger<BitstampTradeClient> logger,
            IBitmapOrderStatusMapper orderStatusMapper)
        {
            _config = config.GetConfigInfo();
            _currencyMapper = currencyMapper;
            _logger = logger;
            _orderStatusMapper = orderStatusMapper;
        }

        public string Name
        {
            get { return "bitstamp"; }
        }

        /// <summary>
        /// Test the method should be in VPN. no China's IP
        /// 
        /// Bitstamp does not support stop order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public MethodResult<OrderResult> MakeANewOrder(OrderRequest order)
        {
            switch (order.OrderType)
            {
                case OrderType.Stop:
                    var errorMesage = "Bitstamp does not support 'stop' order";
                    _logger.LogCritical(errorMesage);
                    return MethodResult<OrderResult>.Failed(errorMesage);
                case OrderType.Limit:
                    return LimitOrder(order);
                case OrderType.Market:
                    return MarketOrder(order);
            }

            return MethodResult<OrderResult>.Failed("Argument tradetype is incorrect!");
        }

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            RateLimit();
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/order_status", Method.POST);
            Authenticate(request);
            request.AddParameter("id", orderId);
            var response = client.Execute<BitstampOrderStatusResponse>(request);
            if (response.IsSuccessful)
            {
                var orderStatus = _orderStatusMapper.GetOrderStatus(response.Data.status);
                return new MethodResult<OrderStatus>() { IsSuccessful = true, Data = orderStatus };
            }
            else
            {
                var errorMessage = $"GetOrderStatus failed, OrderId:{orderId} remote:{response.ErrorMessage}";
                _logger.LogCritical(errorMessage);
                return MethodResult<OrderStatus>.Failed(errorMessage);
            }

        }

        public void GetBlance()
        {
            RateLimit();
            var client = new RestClient(_baseUrlV2);
            var request = new RestRequest("/balance/", Method.POST);

            Authenticate(request);

            var response = client.Execute<BitstampBlance>(request);
            var data = response.Data;
        }

        private MethodResult<OrderResult> LimitOrder(OrderRequest order)
        {
            RateLimit();
            var tradeType = order.TradeType == TradeType.Buy ? "buy" : "sell";
            var currencyPair = _currencyMapper.GetPairName(order.CurrencyPair);
            var resource = $"{tradeType}/{currencyPair}/";
            var client = new RestClient(_baseUrlV2);
            var request = new RestRequest(resource, Method.POST);
            Authenticate(request);
            request.AddParameter("amount", order.Volume.ToString(_enCulture));
            request.AddParameter("price", order.Price.ToString(_enCulture));
            //request.AddParameter("limit_price", order.Price.ToString(_enCulture)); 

            var response = client.Execute<BitstampOrderResponse>(request);
            if (response.IsSuccessful)
            {
                var data = response.Data;
                if (data.status == "error")
                {
                    var errorMessage = $"Bistamp MarketOrder failed, Order:{order} remote:{data.reason}";
                    _logger.LogCritical(errorMessage);
                    return MethodResult<OrderResult>.Failed(errorMessage);
                }
                return new MethodResult<OrderResult>() { IsSuccessful = true, Data = data.Convert() };
            }
            else
            {
                var errorMessage = $"MarketOrder failed, Order:{order} remote:{response.ErrorMessage}";
                _logger.LogCritical(errorMessage);
                return MethodResult<OrderResult>.Failed(errorMessage);
            }
        }

        private MethodResult<OrderResult> MarketOrder(OrderRequest order)
        {
            RateLimit();
            var tradeType = order.TradeType == TradeType.Buy ? "buy" : "sell";
            var currencyPair = _currencyMapper.GetPairName(order.CurrencyPair);
            var resource = $"{tradeType}/market/{currencyPair}/";
            var client = new RestClient(_baseUrlV2);
            var request = new RestRequest(resource, Method.POST);
            Authenticate(request);
            request.AddParameter("amount", order.Volume.ToString(_enCulture));
            var response = client.Execute<BitstampOrderResponse>(request);
            if (response.IsSuccessful)
            {
                return new MethodResult<OrderResult>() { IsSuccessful = true, Data = response.Data.Convert() };
            }
            else
            {
                var errorMessage = $"MarketOrder failed, Order:{order} remote:{response.ErrorMessage}";
                _logger.LogCritical(errorMessage);
                return MethodResult<OrderResult>.Failed(errorMessage);
            }
        }

        private void Authenticate(RestRequest request)
        {
            var nonce = GetNonce();
            string toHash = nonce + _config.CustomerId + _config.ApiKey;
            string signature = CryptoHelper.HmacSha256Hex(_config.Secret, toHash);
            request.AddParameter("key", _config.ApiKey);
            request.AddParameter("nonce", nonce);
            request.AddParameter("signature", signature);
        }

        private string GetNonce()
        {
            var d = TimeHelper.GetTimeStamp() * 100000;
            return d.ToString();
        }

        private DateTime _lastTime = DateTime.UtcNow;
        private void RateLimit()
        {
            var ts = DateTime.UtcNow - _lastTime;
            if (ts.TotalMilliseconds < 100)
            {
                Thread.Sleep(ts);
            }
        }
    }
}
