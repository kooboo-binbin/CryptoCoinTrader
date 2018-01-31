using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            Assert.True(client.Config(GdaxConfigResource.ConfigJson));
        }

        [Fact]
        public void TestJson()
        {
            var accounts = JsonConvert.DeserializeObject<List<GdaxAccount>>(GdaxConfigResource.AccountsJson);
            Assert.Equal(2, accounts.Count);
            var first = accounts[0];
            Assert.Equal("0.0000000000000000", first.Balance);
            Assert.Equal("0.0000000000000000", first.Available);
            Assert.Equal("0.0000000000000000", first.Hold);
            Assert.Equal("BTC", first.Currency);
            Assert.Equal("71452118-efc7-4cc4-8780-a5e22d4baa53", first.Id);
            Assert.Equal("75da88c5-05bf-4f54-bc85-5c775bd68254", first.ProfileId);
        }
    }
}
