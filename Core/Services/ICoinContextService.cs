using CryptoCoinTrader.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    /// <summary>
    /// We have not create each coin context for each thread. 
    /// So when App is running we will get exception. So we use this for instead of it.
    /// </summary>
    public interface ICoinContextService
    {
        CoinContext GetContext();
        void Execute(Action<CoinContext> action);
    }
}
