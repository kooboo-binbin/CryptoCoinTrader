using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Data.Entities;
using System.Linq;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Manifest.Enums;
using AutoMapper;

namespace CryptoCoinTrader.Core.Services.Observations
{
    public class ObservationService : IObservationService
    {
        private readonly static object _lock = new object();
        private readonly CoinContext _coinContext;
        
        private readonly IMapper _mapper;
        private static List<Observation> _observations;


        public ObservationService(CoinContext coinContext,IMapper mapper)
        {
            _coinContext = coinContext;
            _mapper = mapper;
        }

        public void Add(Observation observation)
        {
            var observations = GetObservations();
            observations.Add(observation);
            _coinContext.Add(observation);
            SaveState();
        }

        public void Update(Observation observation)
        {
            var observations = GetObservations();
            var old = observations.FirstOrDefault(it => it.Id == observation.Id);
            _mapper.Map(observation, old);
            SaveState();
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
            return _coinContext.Observations.ToList();
        }

        public void SubtractAvailabeVolume(Guid id, decimal volume)
        {
            var item = GetObservations().FirstOrDefault(it => it.Id == id);
            item.AvailabeVolume -= volume;
            if (item.AvailabeVolume <= 0)
            {
                item.RunningStatus = RunningStatus.Done;
            }
            Task.Run(() =>
            {
                SaveState();
            });
        }

        public void ResetVolume()
        {
            var observations = GetObservations();
            foreach (var item in observations)
            {
                item.AvailabeVolume = 0;
            }
            Task.Run(() =>
            {
                SaveState();
            });
        }

        public void Delete(Guid observationId)
        {
            var observations = GetObservations();
            var item = observations.FirstOrDefault(it => it.Id == observationId);
            if (item != null)
            {
                observations.Remove(item);
                _coinContext.Remove(item);
                SaveState();
            }
        }

        public void SaveState()
        {
            lock (_lock)
            {
                _coinContext.SaveChanges();
            }
        }
    }
}
