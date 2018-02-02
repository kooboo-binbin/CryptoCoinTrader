using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Workers
{
    public interface IWorker
    {
        void Work(List<Observation> observations);
    }
}
