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

        public DateTime DateBuyExecuted { get; set; }

        public DateTime DateSellExecuted { get; set; }

        public bool Bought { get; set; }

        public bool Sold { get; set; }
    }
}
