using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CryptoCoinTrader.Core.Data.Entities
{
    public class Arbitrage
    {
        [Key]
        public Guid Id { get; set; }

        public Guid ObservationId { get; set; }

        public decimal Volume { get; set; }

        public DateTime DateCreated { get; set; }

        public CurrencyPair CurrencyPair { get; set; }

        [StringLength(50)]
        public string ObservationName { get; set; }

    }
}
