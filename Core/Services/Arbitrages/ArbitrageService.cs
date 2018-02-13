using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
using System.Linq;

namespace CryptoCoinTrader.Core.Services.Arbitrages
{
    public class ArbitrageService : IArbitrageService
    {
        private readonly ICoinContextService _coinContextService;
        private readonly static Dictionary<Guid, Arbitrage> _lastArbitrageDict = new Dictionary<Guid, Arbitrage>();
        private readonly CoinContext _coinContext;

        public ArbitrageService(ICoinContextService coinContextService, CoinContext coinContext)
        {
            _coinContextService = coinContextService;
            _coinContext = coinContext;
        }

        public void Add(Arbitrage arbitrage)
        {
            _lastArbitrageDict[arbitrage.ObservationId] = arbitrage; //Get a null excption? 2018-2-13
            using (var context = _coinContextService.GetContext())
            {
                context.Add(arbitrage);
                context.SaveChanges();
            }

        }

        public Arbitrage GetLastOne(Guid observationId)
        {
            return _lastArbitrageDict.GetValueOrDefault(observationId, null);
        }

        public List<Arbitrage> GetList(Guid observationid)
        {
            using (var context = _coinContextService.GetContext())
            {
                return context.Arbitrages.Where(it => it.ObservationId == observationid).ToList();
            }
        }

        public IQueryable<Arbitrage> GetQuery()
        {
            return _coinContext.Arbitrages.AsQueryable();
        }

    }
}
