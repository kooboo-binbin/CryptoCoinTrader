using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Bl3p;
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
        private readonly List<string> _exchangeNames = new List<string>();

        public ExchangeTradeService(IGdaxTradeClient gdax, IBitstampTradeClient bitstamp, IBl3pTradeClient bl3p)
        {
            var exchanges = new List<IExchangeTrade>() { bitstamp, gdax, bl3p };
            foreach (var exchange in exchanges)
            {
                var name = exchange.Name.ToLower();
                _tradeDict.Add(name, exchange);
                _exchangeNames.Add(name);
            }
        }

        public List<string> GetExchangeNames()
        {
            return _exchangeNames;
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
