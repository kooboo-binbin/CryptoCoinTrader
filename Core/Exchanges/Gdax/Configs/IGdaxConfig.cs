using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Gdax.Configs
{
    public interface IGdaxConfig
    {
        GdaxConfigInfo GetConfigInfo();
    }
}
