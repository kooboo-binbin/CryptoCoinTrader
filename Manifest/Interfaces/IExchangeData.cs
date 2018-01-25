using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchangeData
    {
        void Register(List<CurrencyPair> paris);
        void Start();


    }
}
