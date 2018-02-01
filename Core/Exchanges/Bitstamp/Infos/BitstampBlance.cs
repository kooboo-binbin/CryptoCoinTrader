﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos
{
    public class BitstampBlance
    {
        public string usd_balance { get; set; }
        public string btc_balance { get; set; }
        public string eur_balance { get; set; }
        public string usd_reserved { get; set; }
        public string btc_reserved { get; set; }
        public string eur_reserved { get; set; }
        public string usd_available { get; set; }
        public string btc_available { get; set; }
        public string eur_available { get; set; }
        public string fee { get; set; }
    }
}
