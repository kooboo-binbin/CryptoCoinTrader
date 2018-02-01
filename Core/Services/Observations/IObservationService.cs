using CryptoCoinTrader.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Services
{
    public interface IObservationService
    {
        List<Observation> GetObservations();
        void ResetVolume();
        void SaveState();
    }
}
