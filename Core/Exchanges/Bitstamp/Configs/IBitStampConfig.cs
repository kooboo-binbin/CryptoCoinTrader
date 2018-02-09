using CryptoCoinTrader.Core.Exchanges.Bitstamp.Infos;
using CryptoCoinTrader.Manifest.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs
{
    public interface IBitstampConfig: IExchangeConfig
    {
        BitstampConfigInfo GetConfigInfo();
    }
}
