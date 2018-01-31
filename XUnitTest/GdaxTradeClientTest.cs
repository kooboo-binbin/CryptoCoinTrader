using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Core.Exchanges.Gdax.Remotes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTest
{
    public class GdaxTradeClientTest
    {
        [Fact]
        public void TestJson()
        {
            var accounts = JsonConvert.DeserializeObject<List<GdaxAccountResponse>>(GdaxConfigResource.AccountsJson);
            Assert.Equal(2, accounts.Count);
            var first = accounts[0];
            Assert.Equal("0.0000000000000000", first.balance);
            Assert.Equal("0.0000000000000000", first.available);
            Assert.Equal("0.0000000000000000", first.hold);
            Assert.Equal("BTC", first.currency);
            Assert.Equal("71452118-efc7-4cc4-8780-a5e22d4baa53", first.id);
            Assert.Equal("75da88c5-05bf-4f54-bc85-5c775bd68254", first.profile_id);
        }
    }
}
