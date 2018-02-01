using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace XUnitTest
{
    public class BitstampCurrencyMapperTest
    {
        [Fact]
        public void TestGetPairName()
        {
            var currencyMapper = new BitstampCurrencyMapper();
            Assert.Equal("btceur", currencyMapper.GetPairName(CurrencyPair.BtcEur));
            Assert.Equal("btcusd", currencyMapper.GetPairName(CurrencyPair.BtcUsd));
            Assert.Equal("ltceur", currencyMapper.GetPairName(CurrencyPair.LtcEur));
        }
    }
}
