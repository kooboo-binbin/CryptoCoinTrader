using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Core.Services.Messages;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Manifest.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace CryptoCoinTrader.Web
{
    public class App
    {
        private readonly ILogger<App> _logger;

        private readonly IExchangeDataService _exchangeDataService;
        private readonly IExchangeTradeService _exchangeTradeService;
        private readonly IObservationService _observationService;
        private readonly IWorker _worker;
        private readonly AppSettings _appSettings;

        public App(ILogger<App> logger,
            AppSettings appSettings,
            IExchangeDataService exchangeDataService,
            IExchangeTradeService exchangeTradeService,
            IObservationService observationService,
            IOpportunityService opportunityService,
            IWorker worker)
        {
            _appSettings = appSettings;
            _logger = logger;
            _exchangeDataService = exchangeDataService;
            _exchangeTradeService = exchangeTradeService;
            _observationService = observationService;

            _worker = worker;
        }

        /// <summary>
        /// Start data service, add default observations
        /// </summary>
        public void Run()
        {
            //Todo: create a service scope for each run times. else if many records are added in to coinContext. the Performance will deteriorate
            var observations = _observationService.GetObservations();
            ///Todo: Improve  if a new observatoin is added. we can register a new currencypair
            var currencyPairs = new List<CurrencyPair>() { CurrencyPair.BtcEur, CurrencyPair.LtcEur };
            _exchangeDataService.Register(currencyPairs);
            _exchangeDataService.Start();
            _worker.Add(observations);

            var wssv = new WebSocketServer(4888);
            wssv.AddWebSocketService<MessageBehavior>("/message");
            wssv.Start();
            var enviromnent = _appSettings.Production ? "production" : "test";
            Console.WriteLine($"{enviromnent} enviroment");
        }
    }
}
