using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IObservationService
    {
        void Add(Observation observation);
        void Update(Observation observation);

        List<Observation> GetObservations();
        void SubtractAvailabeVolume(Guid id, decimal volume);

        void Delete(Guid observationId);
    }
}
