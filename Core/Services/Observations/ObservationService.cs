using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Data.Entities;
using System.Linq;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Manifest.Enums;

namespace CryptoCoinTrader.Core.Services.Observations
{
    public class ObservationService : IObservationService
    {
        private readonly static object _lock = new object();
        private readonly CoinContext _coinContext;

        private static List<Observation> _observations;
        private static bool isOld = false;


        public ObservationService(CoinContext coinContext)
        {
            _coinContext = coinContext;
        }

        public void Add(Observation observation)
        {
            lock (_lock)
            {
                _coinContext.Add(observation);
                SaveState();
            }

        }

        public void Update(Observation observation)
        {
            lock (_lock)
            {
                var old2 = GetObservatoinsFromDatabase().FirstOrDefault(it => it.Id == observation.Id);
                observation.AssignTo(old2);
                SaveState();
            }
        }

        public List<Observation> GetObservations()
        {
            lock (_lock)
            {
                if (_observations != null && !isOld)
                {
                    return _observations;
                }
                else
                {
                    _observations = GetObservatoinsFromDatabase();
                    return _observations;
                }
            }
        }

        private List<Observation> GetObservatoinsFromDatabase()
        {
            return _coinContext.Observations.Where(it => !it.Deleted).OrderBy(it => it.DateCreated).ToList();
        }

        public void SubtractAvailabeVolume(Guid id, decimal volume)
        {
            lock (_lock)
            {
                var item = GetObservatoinsFromDatabase().FirstOrDefault(it => it.Id == id);
                item.AvailabeVolume -= volume;
                if (item.AvailabeVolume <= 0)
                {
                    item.RunningStatus = RunningStatus.Done;
                }

                SaveState();
            }
        }



        public void Delete(Guid observationId)
        {
            lock (_lock)
            {
                var observations = GetObservatoinsFromDatabase();
                var item = observations.FirstOrDefault(it => it.Id == observationId);
                if (item != null)
                {
                    item.Deleted = true;
                    SaveState();
                }
            }
        }

        public void SaveState()
        {
            isOld = true;
            _coinContext.SaveChanges();

        }
    }
}
