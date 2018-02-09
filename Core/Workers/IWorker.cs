using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Workers
{
    public interface IWorker
    {
        void Add(List<Observation> observations);
        void Add(Observation observation);
        void Delete(Guid observationId);
        void Start();
        void Stop();
        bool GetStatus();
    }
}
