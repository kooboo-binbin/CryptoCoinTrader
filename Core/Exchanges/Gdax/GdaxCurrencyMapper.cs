using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CryptoCoinTrader.Core.Exchanges.Gdax
{
    public class GdaxCurrencyMapper : IGdaxCurrencyMapper
    {
        private static List<KeyValuePair<CurrencyPair, string>> _maps = new List<KeyValuePair<CurrencyPair, string>>() {
            new KeyValuePair<CurrencyPair, string>(CurrencyPair.BtcEur,"BTC-EUR"),
            new KeyValuePair<CurrencyPair, string>(CurrencyPair.BtcUsd,"BTC-ESD"),
            new KeyValuePair<CurrencyPair, string>(CurrencyPair.LtcEur,"LTC-EUR"),
        };

        private static Lazy<Dictionary<CurrencyPair, string>> _enumToStringMaps = new Lazy<Dictionary<CurrencyPair, string>>(() =>
        {
            var temp = new Dictionary<CurrencyPair, string>();
            foreach (var item in _maps)
            {
                temp.Add(item.Key, item.Value);
            }
            return temp;
        });


        private static Lazy<Dictionary<string, CurrencyPair>> _stringToEnumMaps = new Lazy<Dictionary<string, CurrencyPair>>(() =>
        {
            var temp = new Dictionary<string, CurrencyPair>();
            foreach (var item in _maps)
            {
                temp.Add(item.Value, item.Key);
            }
            return temp;
        });


        public string GetPairName(CurrencyPair pair)
        {
            if (_enumToStringMaps.Value.TryGetValue(pair, out string value))
            {
                return value;
            }
            throw new ArgumentOutOfRangeException($"{pair} is not defined in Gdax exchange");
        }


        public CurrencyPair GetCurrency(string gdaxPair)
        {
            return _stringToEnumMaps.Value[gdaxPair];
        }
    }
}
