using CryptoCoinTrader.Core.Exchanges.Bl3p.Infos;
using CryptoCoinTrader.Manifest.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p.Configs
{
    public interface IBl3pConfig : IExchangeConfig
    {
        Bl3pConfigInfo GetConfigInfo();
    }
}
