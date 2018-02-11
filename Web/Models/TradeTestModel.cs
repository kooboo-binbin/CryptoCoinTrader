using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Models
{
    public class TradeTestModel
    {
        public string ExchangeName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyPair CurrencyPair { get; set; }
      
        public decimal Volume { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType TradeType { get; set; }
    }
}
