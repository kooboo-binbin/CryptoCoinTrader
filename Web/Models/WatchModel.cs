using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Models
{
    public class WatchModel
    {
        public Observation Observation { get; set; }
        public WatchOrderBookModel OrderBook { get; set; }
    }
}
