using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Interfaces;
using CryptoCoinTrader.Manifest.Trades;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public class ExchangeTradeService : IExchangeTradeService
    {
        private Dictionary<string, IExchangeTrade> _tradeDict = new Dictionary<string, IExchangeTrade>();
        public ExchangeTradeService(IGdaxTradeClient gdaxTrade, IBitstampTradeClient bitstampTrade)
        {
            _tradeDict.Add(gdaxTrade.Name.ToLower(), gdaxTrade);
            _tradeDict.Add(bitstampTrade.Name.ToLower(), bitstampTrade);
        }

        public MethodResult<OrderResult> MakeANewOrder(string name, OrderRequest order)
        {
            return _tradeDict[name.ToLower()].MakeANewOrder(order);
        }

        public MethodResult<OrderStatus> GetOrderStatus(string name, string orderId)
        {
            return _tradeDict[name.ToLower()].GetOrderStatus(orderId);
        }
    }
}
