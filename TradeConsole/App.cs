using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Exchanges.Bitstamp;
using CryptoCoinTrader.Core.Exchanges.Bitstamp.Configs;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Exchanges.Gdax.Configs;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using CryptoCoinTrader.Manifest.Trades;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TradeConsole
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly ISelfInspectionService _selfInspectionService;
        private readonly IBitstampTradeClient _bitstampTradeClient;
        private readonly IGdaxTradeClient _gdaxTradeClient;
        private readonly IBitstampDataClient _bitstampDataClient;
        private readonly IGdaxDataClient _gdaxDataClient;

        public App(ILogger<App> logger,
            ISelfInspectionService selfInspectionService,
            IBitstampTradeClient bitStampTradeClient,
            IGdaxTradeClient gdaxTradeClient,
            IBitstampDataClient bitstampDataClient,
            IGdaxDataClient gdaxDataClient)
        {
            _logger = logger;
            _selfInspectionService = selfInspectionService;
            _bitstampTradeClient = bitStampTradeClient;
            _gdaxTradeClient = gdaxTradeClient;

            _bitstampDataClient = bitstampDataClient;
            _gdaxDataClient = gdaxDataClient;
        }

        public void Run()
        {
            var inspectionResult = _selfInspectionService.Inspect();
            if (!inspectionResult.IsSuccessful)
            {
                Console.WriteLine(inspectionResult.Message);
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
                return;
            }

            TestBuyBistamp();

            var currency1 = CurrencyPair.BtcUsd;
            _bitstampDataClient.Register(new List<CurrencyPair>() { currency1 });
            _bitstampDataClient.Start();

            _gdaxDataClient.Register(new List<CurrencyPair>() { currency1 });
            _gdaxDataClient.Start();

            WatchSpread(_bitstampDataClient, _gdaxDataClient, currency1);

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
            _gdaxTradeClient.MakeANewOrder(reqeust);
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
            _bitstampTradeClient.MakeANewOrder(reqeust);
        }

        private static void WatchSpread(IBitstampDataClient client, IGdaxDataClient client2, CurrencyPair currencyPair)
        {
            Console.Clear();
            var task = Task.Run(() =>
            {
                while (true)
                {
                    var book1 = client.GetOrderBook(currencyPair);
                    var book2 = client2.GetOrderBook(currencyPair);
                    Console.SetCursorPosition(0, 2);
                    Console.WriteLine("------------Bid----BitStamp--------btceur-------------Ask--------------");
                    Console.WriteLine(book1.ToString(3));
                    Console.WriteLine("------------Bid----GDax------------btceur-------------Ask---------------");
                    Console.WriteLine(book2.ToString(3));


                    if (book1.Bids.Count > 0 && book2.Asks.Count > 0)
                    {
                        var bid0 = book2.Bids[0];
                        var ask0 = book1.Asks[0];
                        var spread = bid0.Price - ask0.Price;
                        Console.WriteLine($"Spread gdx.bid1 {bid0.Price:f2} - bitstmap.ask1 {ask0.Price:f2} = {spread:f2} volume:{Math.Min(bid0.Volume, ask0.Volume)}");
                        if (spread > 3)
                        {
                            arbitarayAmount++;
                            Console.WriteLine("We can do a arbitrary {0}", arbitarayAmount);
                        }
                        else
                        {
                            Console.WriteLine("We can't do a arbitrary");
                        }
                    }

                    Thread.Sleep(300);
                }
            });
        }

        private static int arbitarayAmount = 0;


    }
}
