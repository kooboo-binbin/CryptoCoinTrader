using CryptoCoinTrader.Core;
using CryptoCoinTrader.Core.Exchanges.BitStamp;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Core.Helpers;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
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
        private readonly AppSettings _appSettings;
        public App(ILogger<App> logger, AppSettings appSettings)
        {
            _logger = logger;
            _appSettings = appSettings;
        }

        public void Run()
        {
            _logger.LogInformation("test");

            if (!IPHelper.Check(_appSettings.WorkingIps))
            {
                Console.WriteLine("IP is incorrect. please use vpn or change the working ips in appsetings.json");
                Console.WriteLine("Press any key to quit");
                Console.ReadKey();
                return;
            }
            var client = new BitStampDataClient();
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


        private static void WatchSpread(BitStampDataClient client, GdaxDataClient client2)
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
