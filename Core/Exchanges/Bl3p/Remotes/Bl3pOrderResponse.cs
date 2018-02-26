using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p.Remotes
{
    public class Bl3pOrderResponse
    {
        public string order_id { get; set; }
        public string label { get; set; }
        public string currency { get; set; }
        public string item { get; set; }
        public string bid { get; set; }
        public string amount { get; set; }
        public string price { get; set; }
        public string status { get; set; }
        public string date { get; set; }

        /// <summary>
        /// Total amount of the  trades that got executed
        /// </summary>
        public string total_amount { get; set; }

        /// <summary>
        /// Total amount in EUR of the trades that  got executed
        /// </summary>
        public string total_spent { get; set; }

        public string total_fee { get; set; }
        public string avg_cost { get; set; }

        //trades
    }
}
