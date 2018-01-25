using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Manifest.Interfaces
{
    public interface IExchange
    {
        Guid Id { get; }
        string Name { get; }
    }
}
