using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;

namespace CryptoCoinTrader.Core.Workers
{
    public class Worker : IWorker
    {
        private readonly IExchangeDataService _exchangeDataService;
        private readonly IMessageService _messageService;
        private readonly IObservationService _observationService;
        private readonly IOpportunityService _opportunityService;

        public Worker(IExchangeDataService exchangeDataService,
            IMessageService messageService,
            IObservationService observationService,
            IOpportunityService opportunityService)
        {
            _exchangeDataService = exchangeDataService;
            _messageService = messageService;
            _observationService = observationService;
            _opportunityService = opportunityService;
        }

        public void Work(List<Observation> observations)
        {
            for (int i = 0; i < observations.Count; i++)
            {
                var observation = observations[i];
                RunObservatoin(i, observation);
            }
        }

        private void RunObservatoin(int i, Observation observation)
        {
            var task = Task.Run(() =>
            {
                while (true)
                {
                    var top = i * 5;
                    var book1 = _exchangeDataService.GetOrderBook(observation.Exchange1Name, observation.CurrencyPair);
                    var book2 = _exchangeDataService.GetOrderBook(observation.Exchange2Name, observation.CurrencyPair);
                    _messageService.Write(top + 0, observation.ToConsole());

                    if (book1.Bids.Count > 0 && book2.Asks.Count > 0)
                    {
                        var ask1_0 = book1.Asks[0];
                        var bid2_0 = book2.Bids[0];
                        var spread = bid2_0.Price - ask1_0.Price;
                        var spreadVolume = Math.Min(bid2_0.Volume, ask1_0.Volume);
                        var spreadMessage = $"Spread {observation.Exchange2Name}.bid1 {bid2_0.Price:f2} - {observation.Exchange1Name}.ask1 {ask1_0.Price:f2} = {spread:f2} volume:{spreadVolume}";
                        _messageService.Write(top + 1, spreadMessage);

                        var canArbitrage = _opportunityService.CheckCurrentPrice(observation, ask1_0.Price, spread);
                        var lastArbitrage = _opportunityService.CheckLastArbitrage(observation.Id);
                        if (canArbitrage & lastArbitrage)
                        {
                            var volume = Math.Min(observation.PerVolume, observation.AvaialbeVolume);
                            volume = Math.Min(volume, spreadVolume);
                            _observationService.SubtractAvailabeVolume(observation.Id, volume);
                            _messageService.Write(top + 2, "Do a fake arbitrage");//Todo:
                        }
                        LastArbitrageMessage(top + 3, canArbitrage, lastArbitrage);
                    }
                    Thread.Sleep(200);
                }
            });
        }

        private void LastArbitrageMessage(int top, bool canArbitrage, bool lastArbitrage)
        {
            if (canArbitrage & !lastArbitrage)
            {
                _messageService.Write(top, "Last arbitrage is not finished.");
            }
            else
            {
                _messageService.Write(top, "                                                                                    ");
            }
        }
    }
}
