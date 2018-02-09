using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Manifest.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Configs
{
    public interface IGdaxConfig:IExchangeConfig
    {
        GdaxConfigInfo GetConfigInfo();
    }
}
