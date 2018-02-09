using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchangeSetting
    {
        string GetSetting(string name);
        void SaveSetting(string name, string settings);
    }
}
