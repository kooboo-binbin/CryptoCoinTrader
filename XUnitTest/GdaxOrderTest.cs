using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Core.Exchanges.Gdax.Remotes;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest
{
    public class GdaxOrderTest
    {
        [Fact]
        public void TestConvert()
        {
            var orderRequst = new OrderRequest();
            orderRequst.ClientOrderId = Guid.NewGuid().ToString();
            orderRequst.CurrencyPair = CurrencyPair.BtcEur;
            orderRequst.OrderType = OrderType.Limit;
            orderRequst.Price = 0.00561m;
            orderRequst.TradeType = TradeType.Buy;
            orderRequst.Volume = 0.00036m;

            var currencyMapper = new GdaxCurrencyMapper();
            var order = GdaxOrderRequest.ConvertFrom(orderRequst, currencyMapper);
            Assert.Equal(orderRequst.ClientOrderId, order.client_oid);
            Assert.Equal("BTC-EUR", order.product_id);
            Assert.Equal("limit", order.type);
            Assert.Equal("0.00561", order.price);
            Assert.Equal("0.00036", order.size);
            Assert.Equal("buy", order.side);
        }
    }
}
