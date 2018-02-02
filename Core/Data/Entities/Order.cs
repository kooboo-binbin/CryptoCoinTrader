using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CryptoCoinTrader.Core.Data.Entities
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }   

        public string RemoteId { get; set; }

        public Guid ArbitrageId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime DateCreatedOn { get; set; }

        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        public CurrencyPair Target { get; set; }

        public string ExchangeName { get; set; }

        public decimal ExecutedVolume { get; set; }

        public string Message { get; set; }

    }
}
