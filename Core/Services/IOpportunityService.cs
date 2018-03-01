using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Workers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IOpportunityService
    {
        bool CheckCurrentPrice(Observation observation, ArbitrageInfo info);
        bool CheckLastArbitrage(Guid observationId);
    }
}
