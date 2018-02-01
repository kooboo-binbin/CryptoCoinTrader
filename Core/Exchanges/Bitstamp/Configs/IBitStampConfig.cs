using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs
{
    public interface IBitstampConfig
    {
        BitstampConfigInfo GetConfigInfo();
    }
}
