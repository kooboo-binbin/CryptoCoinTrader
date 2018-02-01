using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CryptoCoinTrader.Core.Data.Entities
{
    public class Observation
    {
        [Key]
        public Guid Id { get; set; }

        public string Exchange1Name { get; set; }

        public string Exchange2Name { get; set; }

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
        /// Per trade volume
        /// </summary>
        public decimal PerVolume { get; set; }

        /// <summary>
        /// the unit is coin
        /// </summary>
        public decimal MaxVolume { get; set; }

        public decimal AvaialbeVolume { get; set; }

        public DateTime DateCreated { get; set; }

        public string ToConsole()
        {
            var spread = SpreadType == SpreadType.Percentage ? SpreadPercentage.ToString("p2") : SpreadValue.ToString("f2");
            var message = $"Id:{Id} \t Exchanges:{Exchange1Name}-{Exchange2Name} \t MaxVolume:{MaxVolume:f2} PerVolume:{PerVolume:f2} \t AvailableVolume:{AvaialbeVolume:f2} \t Spread:{spread} ";
            return message;
        }
    }
}
