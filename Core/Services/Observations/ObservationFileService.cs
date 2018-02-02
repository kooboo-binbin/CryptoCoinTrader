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
        private readonly object _lock = new object();
        private List<Observation> _observations;

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
                    MaxVolume = 100.00m,
                    AvaialbeVolume = 100.00m,
                    PerVolume = 6,
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
            item.AvaialbeVolume -= volume;
            if (item.AvaialbeVolume <= 0)
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
                item.AvaialbeVolume = 0;
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
    }
}
