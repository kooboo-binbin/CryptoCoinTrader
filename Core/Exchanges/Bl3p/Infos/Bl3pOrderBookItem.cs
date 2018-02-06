using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p.Infos
{
    /// <summary>
    /// The unit is int. we should divide it by  100000
    /// </summary>
    public class Bl3pOrderBookItem
    {
        [JsonProperty("price_int")]
        public decimal Price { get; set; }

        [JsonProperty("amount_int")]
        public decimal Amount { get; set; }
    }
}
