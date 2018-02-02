using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Threading;
using RestSharp;
using Newtonsoft.Json;
using CryptoCoinTrader.Manifest.Trades;
using CryptoCoinTrader.Manifest.Helpers;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Manifest;
using System.Diagnostics;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using System.Net;
using CryptoCoinTrader.Core.Exchanges.Gdax.Remotes;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Exchanges.Gdax
{
    public class GdaxTradeClient : IGdaxTradeClient
    {
        //url https://docs.gdax.com/?javascript#list-fills
        private readonly string baseUrl = "https://api.gdax.com";
        private readonly GdaxConfigInfo _config;
        private readonly IGdaxCurrencyMapper _currencyMapper;
        private readonly IGdaxOrderStatusMapper _orderStatusMapper;

        public GdaxTradeClient(IGdaxConfig gdaxConfig,
            IGdaxCurrencyMapper currencyMapper,
            IGdaxOrderStatusMapper orderStatusMapper)
        {
            _config = gdaxConfig.GetConfigInfo();
            _currencyMapper = currencyMapper;
            _orderStatusMapper = orderStatusMapper;
        }

        public string Name
        {
            get { return "Gdax"; }
        }

        public MethodResult<OrderResult> MakeANewOrder(OrderRequest order)
        {
            RateLimit();
            var request = new RestRequest("/orders", Method.POST);
            request.AddJsonBody(GdaxOrderRequest.ConvertFrom(order, _currencyMapper));
            AddAuthenticationHeader(request);
            var client = new RestClient(baseUrl);
            //client.Proxy = new WebProxy("127.0.0.1", 8888); for fiddler trace
            var response = client.Execute<GdaxOrderResponse>(request);
            if (response.IsSuccessful)
            {
                var result = new OrderResult();
                return new MethodResult<OrderResult>()
                {
                    IsSuccessful = true,
                    Data = response.Data.Convert()
                };
            }
            return MethodResult<OrderResult>.Failed(response.ErrorMessage);
        }

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            RateLimit();
            var request = new RestRequest($"/orders/{orderId}", Method.GET);
            AddAuthenticationHeader(request);
            var client = new RestClient(baseUrl);
            var response = client.Execute<GdaxOrderResponse>(request);
            if (response.IsSuccessful)
            {
                return new MethodResult<OrderStatus>() { IsSuccessful = true, Data = _orderStatusMapper.GetOrderStatus(response.Data.status) };
            }
            return MethodResult<OrderStatus>.Failed(response.ErrorMessage);
        }

        public void GetAccounts()
        {
            RateLimit();
            var request = new RestRequest("/accounts", Method.GET);
            AddAuthenticationHeader(request);
            var client = new RestClient(baseUrl);
            var response = client.Execute<List<GdaxAccountResponse>>(request);
            var accounts = response.Data;

        }

        private void AddAuthenticationHeader(RestRequest request)
        {
            var body = request.Parameters.FirstOrDefault(it => it.Type == ParameterType.RequestBody)?.Value.ToString();
            // gdax is funny and wants a seconds double for the nonce, weird... we convert it to double and back to string invariantly to ensure decimal dot is used and not comma
            string timestamp = TimeHelper.GetTimeStamp().ToString();// double.Parse(payload["nonce"].ToString()).ToString(CultureInfo.InvariantCulture);
            //payload.Remove("nonce");
            string form = body ?? "";
            string toHash = timestamp + request.Method.ToString().ToUpper() + request.Resource + form;
            string signatureBase64String = CryptoHelper.HmacSha256Base64(_config.Secret, toHash);

            request.AddHeader("CB-ACCESS-KEY", _config.Key);
            request.AddHeader("CB-ACCESS-SIGN", signatureBase64String);
            request.AddHeader("CB-ACCESS-TIMESTAMP", timestamp);
            request.AddHeader("CB-ACCESS-PASSPHRASE", _config.Passphrase);
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
