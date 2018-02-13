using CryptoCoinTrader.Core.Data.Enums;
using MassTransit;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CryptoCoinTrader.Core.Data.Entities
{
    public class Log
    {
        public Log()
        {
            Id = NewId.NextGuid();
        }

        [Key]
        public Guid Id { get; set; }

        public Guid? ObservatoinId { get; set; }

        [StringLength(50)]
        public string ObservationName { get; set; }

        public string Message { get; set; }

        public DateTime DateCreated { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogType LogType { get; set; }
    }
}
