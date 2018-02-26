using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CryptoCoinTrader.Core.Exchanges.Bl3p.Configs;
using CryptoCoinTrader.Core.Exchanges.Bl3p.Infos;
using CryptoCoinTrader.Core.Exchanges.Bl3p.Remotes;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Helpers;
using CryptoCoinTrader.Manifest.Trades;
using RestSharp;
using System.Net;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{
    public class Bl3pTradeClient : IBl3pTradeClient
    {
        private readonly Bl3pConfigInfo _bl3PConfigInfo;
        private readonly IBl3pCurrencyMapper _currencyMapper;
        private readonly string _baseUrl = "https://api.bl3p.eu/1/";
        private readonly Dictionary<string, OrderStatus> _orderStatusMapper = new Dictionary<string, OrderStatus>() {
            { "pending", OrderStatus.Pending},
            { "open",OrderStatus.Open},
            { "closed",OrderStatus.Finished}, //unsure
            { "cancelled",OrderStatus.Unknowed},
            { "placed",OrderStatus.Finished} //unsure //We sure test it
        };

        public Bl3pTradeClient(IBl3pConfig config, IBl3pCurrencyMapper currencyMapper)
        {
            _bl3PConfigInfo = config.GetConfigInfo();
            _currencyMapper = currencyMapper;

        }

        public string Name => Constants.Name;

        public MethodResult<OrderStatus> GetOrderStatus(string orderId)
        {
            var temp = orderId.Split('|');
            var market = temp[0];
            var call = $"{market}/money/order/result";
            var url = $"{_baseUrl}{call}";
            var client = new RestClient(url);
            //client.Proxy = new WebProxy("127.0.0.1", 8888); //for fiddler trace
            var request = new RestRequest(Method.POST);
            request.AddParameter("order_id", temp[1]);
            request.AddParameter("nonce", GetNonce());
            Authenticate(call, request);
            var result = client.Execute<Bl3pOrderResponse>(request);
          
            if (result.IsSuccessful)
            {
                return new MethodResult<OrderStatus>()
                {
                    IsSuccessful = false,
                    Data = _orderStatusMapper[result.Data.status],
                    Message = result.ErrorMessage
                };
            }

            return new MethodResult<OrderStatus>() { IsSuccessful = false, Data = OrderStatus.Unknowed, Message = result.ErrorMessage };
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>

        public MethodResult<OrderResult> MakeANewOrder(OrderRequest order)
        {
            var market = _currencyMapper.GetPairName(order.CurrencyPair);
            var call = $"{market}/money/order/add";
            var url = $"{_baseUrl}{call}";
            var client = new RestClient(url);

            var request = new RestRequest(Method.POST);
            var type = order.TradeType == TradeType.Buy ? "bid" : "ask";
            var amount_int = order.Volume * 100000000; //1e8

            request.AddParameter("type", type);
            request.AddParameter("amount_int", amount_int.ToString("f0"));
            if (order.OrderType == OrderType.Limit)
            {
                var price_int = order.Price * 100000;
                request.AddParameter("price_int", price_int.ToString("f0"));
            }
            request.AddParameter("fee_currency", "EUR");
            request.AddParameter("nonce", GetNonce());

            Authenticate(call, request);
            var result = client.Execute<Bl3pOrderCreatedResponse>(request);
            if (result.IsSuccessful)
            {
                return new MethodResult<OrderResult>()
                {
                    IsSuccessful = true,
                    //Because the function get orderstatus needs market, so we need to save the makrtet in the orderId
                    Data = new OrderResult() { Id = $"{market}|{result.Data.order_id}", Status = OrderStatus.Active }
                };
            }
            else
            {
                return new MethodResult<OrderResult>()
                {
                    IsSuccessful = false,
                    Message = result.ErrorMessage,
                    Data = new OrderResult() { }
                };
            }
        }

        private string GetNonce()
        {
            var d = TimeHelper.GetTimeTicks();
            return d.ToString();
        }

        /// <summary>
        /// CallPath+\0+ querystring
        /// </summary>
        private void Authenticate(string call, RestRequest request)
        {
            var querys = request.Parameters.Where(it => it.Type == ParameterType.GetOrPost).Select(it => $"{it.Name}={it.Value}");
            var queryString = string.Join("&", querys);
            var message = $"{call}\0{queryString}";
            var sign = CryptoHelper.HmacSha512Base64(_bl3PConfigInfo.PrivateKey, message);
            request.AddHeader("Rest-Key", _bl3PConfigInfo.PublicKey);
            request.AddHeader("Rest-Sign", sign);
            request.AddHeader("Content-Type", "application/json");
        }
    }

}
