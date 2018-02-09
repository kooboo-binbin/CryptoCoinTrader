using CryptoCoinTrader.Manifest;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Exchanges
{
    public interface IExchangeConfigService
    {
        void GetConfigs();
        MethodResult Validate(string name, string json);
        
    }
}
