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

        public App(ILogger<App> logger,
            ISelfInspectionService selfInspectionService,
            IBitstampTradeClient bitStampTradeClient,
            IGdaxTradeClient gdaxTradeClient)
        {
            _logger = logger;
            _selfInspectionService = selfInspectionService;
            _bitstampTradeClient = bitStampTradeClient;
            _gdaxTradeClient = gdaxTradeClient;
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


            _gdaxTradeClient.GetAccounts();


            var client = new BitstampDataClient();
            client.Register(new List<CurrencyPair>() { CurrencyPair.BtcEur });
            client.TickerChanged += Client_TradeChanged;
            client.OrderBookChanged += Client_OrderBookChanged;
            client.Start();

            var client2 = new GdaxDataClient();
            client2.OrderBookChanged += Client_OrderBookChanged;
            client2.Register(new List<CurrencyPair>() { CurrencyPair.BtcEur });
            client2.Start();

            WatchSpread(client, client2);

            Console.ReadLine();
        }


        private static void WatchSpread(BitstampDataClient client, GdaxDataClient client2)
        {
            Console.Clear();
            var task = Task.Run(() =>
            {
                while (true)
                {
                    var book1 = client.GetOrderBook(CurrencyPair.BtcEur);
                    var book2 = client2.GetOrderBook(CurrencyPair.BtcEur);
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

        private static void Client_OrderBookChanged(CurrencyPair arg1, OrderBook orderbook)
        {

            //Console.SetCursorPosition(0, 2);

            // Console.WriteLine(orderbook);
        }

        private static void Client_TradeChanged(CurrencyPair obj, Ticker ticker)
        {
            Console.WriteLine(ticker);
        }
    }
}
