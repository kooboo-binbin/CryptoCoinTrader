using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using CryptoCoinTrader.Manifest.Trades;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services.Arbitrages;
using CryptoCoinTrader.Core.Services.Orders;
using CryptoCoinTrader.Core.Workers;

namespace TradeConsole
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly ISelfInspectionService _selfInspectionService;

        private readonly IExchangeDataService _exchangeDataService;
        private readonly IExchangeTradeService _exchangeTradeService;
        private readonly IObservationService _observationService;
        private readonly IWorker _worker;

        public App(ILogger<App> logger,
            ISelfInspectionService selfInspectionService,
            IExchangeDataService exchangeDataService,
            IExchangeTradeService exchangeTradeService,
            IObservationService observationService,
            IOpportunityService opportunityService,
            IWorker worker)
        {
            _logger = logger;
            _selfInspectionService = selfInspectionService;
            _exchangeDataService = exchangeDataService;
            _exchangeTradeService = exchangeTradeService;
            _observationService = observationService;

            _worker = worker;
        }

        public void Run()
        {
            Console.Clear();

            var inspectionResult = _selfInspectionService.Inspect();
            if (!inspectionResult.IsSuccessful)
            {
                Console.WriteLine(inspectionResult.Message);
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Prease any key to start it");
            Console.ReadKey();
            Console.Clear();

            var observations = _observationService.GetObservations();
            var currencyPairs = observations.Select(it => it.CurrencyPair).ToList();
            _exchangeDataService.Register(currencyPairs);
            _exchangeDataService.Start();
            _worker.Add(observations);

            Console.ReadLine();
            Console.ReadLine();
        }

        private void TestBuyGdax()
        {
            var reqeust = new OrderRequest() { };
            reqeust.ClientOrderId = Guid.NewGuid().ToString();
            reqeust.CurrencyPair = CurrencyPair.LtcEur;
            reqeust.OrderType = OrderType.Limit;  //Market has not been tested
            reqeust.Price = 1m;
            reqeust.TradeType = TradeType.Buy;
            reqeust.Volume = 0.1m;
            _exchangeTradeService.MakeANewOrder("gdax", reqeust);
        }

        private void TestBuyBistamp()
        {
            ///Minmum order size is 5 euro
            var reqeust = new OrderRequest() { };
            reqeust.ClientOrderId = Guid.NewGuid().ToString();
            reqeust.CurrencyPair = CurrencyPair.LtcEur;
            reqeust.OrderType = OrderType.Limit;  //Market has not been tested
            reqeust.Price = 5m;
            reqeust.TradeType = TradeType.Buy;
            reqeust.Volume = 1m;
            _exchangeTradeService.MakeANewOrder("bitstamp", reqeust);
        }

    }
}
