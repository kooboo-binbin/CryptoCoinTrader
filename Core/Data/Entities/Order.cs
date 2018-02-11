using CryptoCoinTrader.Manifest.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        public Guid ObservationId { get; set; }

        public string ExchangeName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatus OrderStatus { get; set; }

        public decimal Price { get; set; }

        public decimal Volume { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CurrencyPair CurrencyPair { get; set; }

        public decimal ExecutedVolume { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

    }
}
