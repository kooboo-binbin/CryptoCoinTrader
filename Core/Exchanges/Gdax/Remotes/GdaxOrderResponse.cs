using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Infos
{
    /// <summary>
    /// ResetClient does not support json property
    /// </summary>
    public class GdaxOrderResponse
    {
        public string id { get; set; }
        public string price { get; set; }
        public string size { get; set; }

        public string product_id { get; set; }
        public string side { get; set; }
        public string stp { get; set; }
        public string limit { get; set; }

  
        public string time_in_force { get; set; }

   
        public bool post_only { get; set; }


        public string created_at { get; set; }

        public string fill_fess { get; set; }

 
        public string filled_size { get; set; }

   
        public string executed_value { get; set; }

        /// <summary>
        /// open pending active
        /// </summary>
        public string status { get; set; }

        public bool settled { get; set; }

        public OrderResult Convert()
        {
            var result = new OrderResult();
            result.Id = this.id;
            var dict = new Dictionary<string, OrderStatus>() { { "open", OrderStatus.Open }, { "pending", OrderStatus.Pending }, { "active", OrderStatus.Active } };
            if (dict.ContainsKey(this.status))
            {
                result.Status = dict[this.status];
            }
            return result;
        }
    }

}
