using CryptoCoinTrader.Manifest.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Models
{
    public class RunningStatusModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public RunningStatus Status { get; set; }
    }
}
