using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IOpportunityService
    {
        bool CheckCurrentPrice(Observation observation, decimal price, decimal spread);
        bool CheckLastArbitrage(Guid observationId);
    }
}
