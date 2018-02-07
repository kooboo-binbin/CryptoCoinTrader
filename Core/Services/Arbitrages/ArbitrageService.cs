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
        private readonly CoinContext _coinContext;
        private Dictionary<Guid, Arbitrage> _lastArbitrageDict = new Dictionary<Guid, Arbitrage>();

        public ArbitrageService(CoinContext coinContext)
        {
            _coinContext = coinContext;
        }

        public void Add(Arbitrage arbitrage)
        {
            _lastArbitrageDict[arbitrage.ObservationId] = arbitrage;

            _coinContext.Add(arbitrage);
            _coinContext.SaveChanges();
        }

        public Arbitrage GetLastOne(Guid observationId)
        {
            return _lastArbitrageDict.GetValueOrDefault(observationId, null);
        }

        public List<Arbitrage> GetList(Guid observationid)
        {
            return _coinContext.Arbitrages.Where(it => it.ObservationId == observationid).ToList();
        }
    }
}
