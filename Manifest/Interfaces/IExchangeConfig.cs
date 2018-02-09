using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchangeConfig
    {
        string Name { get; }
        MethodResult Valid(string json);
        string GetDefaultJson();
    }
}
