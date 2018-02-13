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

        private readonly ICoinContextService _coinContextService;
        private static List<Observation> _observations;
       


        public ObservationService(ICoinContextService coinContextService)
        {
            _coinContextService = coinContextService;
        }

        public void Add(Observation observation)
        {
            _observations.Add(observation);
            Task.Run(() =>
            {
                using (var context = _coinContextService.GetContext())
                {
                    context.Add(observation);
                    context.SaveChanges();
                }
            });
        }

        public void Update(Observation observation)
        {
            var old = GetObservations().FirstOrDefault(it => it.Id == observation.Id);
            observation.AssignTo(old);
            UpdateDatabase(observation);
        }

        private void UpdateDatabase(Observation observation)
        {
            Task.Run(() =>
            {
                ExecuteInDatabase((item) =>
                {
                    observation.AssignTo(item);
                }, observation.Id);
            });
        }

        public List<Observation> GetObservations()
        {
            if (_observations != null)
            {
                return _observations;
            }
            else
            {
                _observations = GetObservatoinsFromDatabase();
                return _observations;
            }
        }

        private List<Observation> GetObservatoinsFromDatabase()
        {
            using (var context = _coinContextService.GetContext())
            {
                return context.Observations.Where(it => !it.Deleted).OrderBy(it => it.DateCreated).ToList();
            }
        }

        public void SubtractAvailabeVolume(Guid id, decimal volume)
        {
            var item = GetObservations().FirstOrDefault(it => it.Id == id);
            item.AvailabeVolume -= volume;

            SubtractAvailabeVolumeDatabase(id, volume);
        }

        private void SubtractAvailabeVolumeDatabase(Guid id, decimal volume)
        {
            Task.Run(() =>
            {
                ExecuteInDatabase((item) =>
                    {
                        item.AvailabeVolume -= volume;
                        if (item.AvailabeVolume <= 0)
                        {
                            item.RunningStatus = RunningStatus.Done;
                        }

                    }, id);
            });
        }


        public void Delete(Guid observationId)
        {
            var item = GetObservations().FirstOrDefault(it => it.Id == observationId);
            if (item != null)
            {
                _observations.Remove(item);
            }
            DeleteDatabase(observationId);
        }

        private void DeleteDatabase(Guid observationId)
        {
            Task.Run(() =>
            {
                ExecuteInDatabase((item) =>
                {
                    if (item != null)
                    {
                        item.Deleted = true;
                    }
                }, observationId);

            });
        }

        private void ExecuteInDatabase(Action<Observation> action, Guid observationId)
        {
            using (var context = _coinContextService.GetContext())
            {
                var item = context.Observations.FirstOrDefault(it => it.Id == observationId);
                action(item);
                context.SaveChanges();
            }
        }
    }
}
