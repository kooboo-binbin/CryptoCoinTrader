using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Remotes
{
    public class GdaxAccountResponse
    {
        public string id { get; set; }

        /// <summary>
        /// BTC,USD
        /// </summary>
        public string currency { get; set; }
        public string balance { get; set; }
        public string available { get; set; }
        public string hold { get; set; }

        public string profile_id { get; set; }
    }
}
