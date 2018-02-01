using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Remotes
{
    public class GdaxSubscribe
    {
        public string Type { get; set; }

        [JsonProperty("product_ids")]
        public List<string> ProductIds { get; set; }

        public List<Object> Channels { get; set; }
    }
}
