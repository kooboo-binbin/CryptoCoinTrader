using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Manifest.Enums;
using Newtonsoft.Json;

namespace CryptoCoinTrader.Core.Services
{
    public class ObservationFileService : IObservationService
    {
        private readonly string fileName = "observations.json";
        private static readonly object _lock = new object();
        private static List<Observation> _observations;

        public void Add(Observation observation)
        {
            var observations = GetObservations();
            observations.Add(observation);
            SaveState();
        }

        public void Update(Observation observation)
        {
            throw new NotImplementedException();
        }

        public List<Observation> GetObservations()
        {
            if (_observations != null)
            {
                return _observations;
            }
            else
            {
                _observations = GetObservatoinsFromFile();
                return _observations;
            }
        }

        private List<Observation> GetObservatoinsFromFile()
        {
            if (File.Exists(fileName))
            {
                using (var sr = new StreamReader(fileName))
                {
                    return JsonConvert.DeserializeObject<List<Observation>>(sr.ReadToEnd());
                }
            }
            else
            {
                var list = new List<Observation>();
                list.Add(new Observation()
                {
                    DateCreated = DateTime.UtcNow,
                    BuyExchangeName = "bitstamp",
                    SellExchangeName = "gdax",
                    Id = Guid.NewGuid(),
                    MaximumVolume = 100.00m,
                    AvailabeVolume = 100.00m,
                    MinimumVolume = 0.05m,
                    PerVolume = 0.03m,
                    SpreadValue = 3m,
                    SpreadPercentage = 0.03m,
                    SpreadType = SpreadType.Percentage,
                    CurrencyPair = CurrencyPair.LtcEur,
                });

                var json = JsonConvert.SerializeObject(list, Formatting.Indented);
                using (var sw = new StreamWriter(fileName))
                {
                    sw.WriteLine(json);
                }
                return new List<Observation>();
            }
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


        public void SaveState()
        {
            lock (_lock)
            {
                var json = JsonConvert.SerializeObject(GetObservations(), Formatting.Indented);
                using (var sw = new StreamWriter(fileName))
                {
                    sw.WriteLine(json);
                }
            }
        }

        public void Delete(Guid observationId)
        {
            var observations = GetObservations();
            var item = observations.FirstOrDefault(it => it.Id == observationId);

            if (item != null)
            {
                observations.Remove(item);
                SaveState();
            }
        }
    }
}
