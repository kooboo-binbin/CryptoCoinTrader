using CryptoCoinTrader.Manifest.Trades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Remotes
{
    public class GdaxOrderRequest
    {
        /// <summary>
        /// Client Orderid
        /// </summary>
        public string client_oid { get; set; }

        /// <summary>
        /// limit,market,stop
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// buy or sell
        /// </summary>
        public string side { get; set; }

        /// <summary>
        /// Mininum size is 0.1
        /// </summary>
        public string size { get; set; }

        public string price { get; set; }

        /// <summary>
        /// BTC-USD
        /// </summary>
        public string product_id { get; set; }

        public static GdaxOrderRequest ConvertFrom(OrderRequest order, IGdaxCurrencyMapper currencyMapper)
        {
            var maps = new Dictionary<OrderType, string>() { { OrderType.Limit, "limit" }, { OrderType.Market, "market" }, /*{ OrderType.Stop, "stop" } */};
            var gdaxOrder = new GdaxOrderRequest();
            gdaxOrder.client_oid = order.ClientOrderId;
            gdaxOrder.type = maps[order.OrderType];
            gdaxOrder.side = order.TradeType == Manifest.Enums.TradeType.Buy ? "buy" : "sell";
            gdaxOrder.size = order.Volume.ToString();
            gdaxOrder.price = order.Price.ToString();
            gdaxOrder.product_id = currencyMapper.GetPairName(order.CurrencyPair);
            return gdaxOrder;
        }
    }
}
