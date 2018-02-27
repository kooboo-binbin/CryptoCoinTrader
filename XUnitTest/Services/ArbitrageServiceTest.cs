using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Arbitrages;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest.Services
{
    public class ArbitrageServiceTest : BaseTest
    {
        [Fact]
        public void AddAndGet()
        {
            using (var context = GetContext())
            {
                var arbitrage = new Arbitrage();
                arbitrage.Id = Guid.NewGuid();
                arbitrage.ObservationId = Guid.NewGuid();
                var coinContextService = new CoinContextService();
                var service = new ArbitrageService(coinContextService, context);
                service.Add(arbitrage);
                var lastOne = service.GetLastOne(arbitrage.ObservationId);
                Assert.Equal(arbitrage, lastOne);
                Assert.Equal(arbitrage.Id, lastOne.Id);
            }
        }
    }
}
