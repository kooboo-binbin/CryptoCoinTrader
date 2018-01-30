using CryptoCoinTrader.Core.Exchanges.Gdax;
using System;
using Xunit;

namespace XUnitTest
{
    public class GdaxTradeClientTest
    {
        [Fact]
        public void TestConfig()
        {
            var client = new GdaxTradeClient();
            Assert.False(client.Config(""));
            Assert.True(client.Config(GdaxConfigResource.Json));
        }
    }
}
