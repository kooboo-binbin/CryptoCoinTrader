using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{
    public class BitstampCurrencyMapper : IBitstampCurrencyMapper
    {
        public CurrencyPair GetCurrency(string gdaxPair)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// btcusd,btceur,ltceur
        /// </summary>
        /// <param name="pair"></param>
        /// <returns></returns>
        public string GetPairName(CurrencyPair pair)
        {
            return pair.ToString().ToLower();
        }
    }
}
