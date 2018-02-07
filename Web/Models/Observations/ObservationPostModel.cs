using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Models.Observations
{
    public class ObservationPostModel
    {
        public string BuyExchangeName { get; set; }

        public string SellExchangeName { get; set; }

        public CurrencyPair CurrencyPair { get; set; }

        /// <summary>
        /// For example 3 euro
        /// </summary>
        public decimal SpreadValue { get; set; }

        /// <summary>
        /// For example 1.5%
        /// </summary>
        public decimal SpreadPercentage { get; set; }

        /// <summary>
        /// By percentage or by Value
        /// By value is 0
        /// By percentage is 1
        /// </summary>
        public SpreadType SpreadType { get; set; }

        /// <summary>
        /// Only the spread volume is greater than our mini volume, then we do a arbitrage
        /// </summary>
        public decimal SpreadMinimumVolume { get; set; }

        /// <summary>
        /// Per trade volume
        /// </summary>
        public decimal PerVolume { get; set; }

        /// <summary>
        /// the unit is coin, How many volume we want to arbitrage at this observatoin
        /// </summary>
        public decimal MaxVolume { get; set; }


        public decimal AvaialbeVolume { get; set; }


    }
}
