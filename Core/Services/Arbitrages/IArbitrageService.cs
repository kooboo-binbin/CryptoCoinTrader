using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoCoinTrader.Core.Services.Arbitrages
{
    public interface IArbitrageService
    {
        Arbitrage GetLastOne(Guid observationId);

        void Add(Data.Entities.Arbitrage arbitrage);

        List<Arbitrage> GetList(Guid observationId);

        IQueryable<Arbitrage> GetQuery();
    }
}
