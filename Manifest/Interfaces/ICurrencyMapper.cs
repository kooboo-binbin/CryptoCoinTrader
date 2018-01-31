using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface ICurrencyMapper
    {
        string GetPairName(CurrencyPair pair);
        CurrencyPair GetCurrency(string pair);
    }
}
