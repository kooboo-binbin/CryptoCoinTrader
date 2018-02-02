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

namespace TradeConsole
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly ISelfInspectionService _selfInspectionService;

        private readonly IExchangeDataService _exchangeDataService;
        private readonly IExchangeTradeService _exchangeTradeService;
        private readonly IObservationService _observationService;
        private readonly IArbitrageService _arbitrageService;

        public App(ILogger<App> logger,
            ISelfInspectionService selfInspectionService,
            IExchangeDataService exchangeDataService,
            IExchangeTradeService exchangeTradeService,
            IObservationService observationService,
            IArbitrageService arbitrageService)
        {
            _logger = logger;
            _selfInspectionService = selfInspectionService;
            _exchangeDataService = exchangeDataService;
            _exchangeTradeService = exchangeTradeService;
            _observationService = observationService;
            _arbitrageService = arbitrageService;
        }

        public void Run()
        {
            Console.Clear();
            Task.Run(() =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    Write(2, $"I am {i}");
                    Thread.Sleep(200);
                };
            });
            Task.Run(() =>
            {
                var i = 0;
                while (true)
                {
                    i++;
                    Write(3, $"she is {i}");
                    Thread.Sleep(200);
                };
            });
            var inspectionResult = _selfInspectionService.Inspect();
            if (!inspectionResult.IsSuccessful)
            {
                Console.WriteLine(inspectionResult.Message);
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
                return;
            }
            var observations = _observationService.GetObservations();
            var currencyPairs = observations.Select(it => it.CurrencyPair).ToList();
            _exchangeDataService.Register(currencyPairs);
            _exchangeDataService.Start();

            WatchObservatoins(observations);


            Console.WriteLine();
            Console.WriteLine("Do you want to start the arbitrage");
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

        private void WatchObservatoins(List<Observation> observations)
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
                    Write(top + 0, observation.ToConsole());

                    if (book1.Bids.Count > 0 && book2.Asks.Count > 0)
                    {
                        var ask1_0 = book1.Asks[0];
                        var bid2_0 = book2.Bids[0];
                        var spread = bid2_0.Price - ask1_0.Price;
                        var spreadVolume = Math.Min(bid2_0.Volume, ask1_0.Volume);
                        var spreadMessage = $"Spread {observation.Exchange2Name}.bid1 {bid2_0.Price:f2} - {observation.Exchange1Name}.ask1 {ask1_0.Price:f2} = {spread:f2} volume:{spreadVolume}";
                        Write(top + 1, spreadMessage);

                        var canArbitrage = CanArbitrage(observation, ask1_0, spread);
                        var lastArbitrage = CheckLastArbitrage(observation.Id);
                        if (canArbitrage & lastArbitrage)
                        {
                            var volume = Math.Min(observation.PerVolume, observation.AvaialbeVolume);
                            volume = Math.Min(volume, spreadVolume);
                            _observationService.SubtractAvailabeVolume(observation.Id, volume);
                            Write(top + 2, "Do a fake arbitrage");//Todo:
                        }
                        LastArbitrageMessage(top + 3, canArbitrage, lastArbitrage);
                    }
                    Thread.Sleep(200);
                }
            });
        }

        private bool CheckLastArbitrage(Guid observationId)
        {
            var arbitrage = _arbitrageService.GetLastOne(observationId);

            return true;
            //Todo:
        }

        private bool CanArbitrage(Observation observation, OrderBookItem ask1_0, decimal spread)
        {
            var canArbitrage = false;
            if (observation.SpreadType == SpreadType.Percentage)
            {
                if ((spread / ask1_0.Price) > observation.SpreadPercentage)
                {
                    canArbitrage = true;
                }
            }
            else
            {
                if (spread > observation.SpreadValue)
                {
                    canArbitrage = true;
                }
            }

            return canArbitrage;
        }

        private void LastArbitrageMessage(int top, bool canArbitrage, bool lastArbitrage)
        {
            if (canArbitrage & !lastArbitrage)
            {
                Write(top, "Last arbitrage is not finished.");
            }
            else
            {
                Write(top, "                                                                                    ");
            }
        }

        private object _consoleLock = new object();
        private void Write(int top, string message)
        {
            lock (_consoleLock)
            {
                Console.SetCursorPosition(0, top);
                Console.WriteLine(message);
            }
        }

    }
}
