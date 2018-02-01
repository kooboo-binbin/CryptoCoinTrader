using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IObservationService
    {
        List<Observation> GetObservations();
        void SubtractAvailabeVolume(Guid id, decimal volume);
        void ResetVolume();
        void SaveState();
    }
}
