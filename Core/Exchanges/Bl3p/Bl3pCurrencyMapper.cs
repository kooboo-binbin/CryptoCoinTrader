using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{
    public class Bl3pCurrencyMapper : IBl3pCurrencyMapper
    {
        private Lazy<Dictionary<string, CurrencyPair>> _stringToEnumMaps = new Lazy<Dictionary<string, CurrencyPair>>(() =>
        {
            return new Dictionary<string, CurrencyPair>() { { "BTCEUR", CurrencyPair.BtcEur }, { "LTCEUR", CurrencyPair.LtcEur } };
        });

        public CurrencyPair GetCurrency(string pair)
        {
            return _stringToEnumMaps.Value[pair];
        }

        public string GetPairName(CurrencyPair pair)
        {
            return pair.ToString().ToUpper();
        }
    }
}
