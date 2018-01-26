using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Exchanges;
using CryptoCoinTrader.Core.Exchanges.BitStamp;
using CryptoCoinTrader.Core.Exchanges.Gdax;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;

namespace TradeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            //var client = new BitStampDataClient();
            //client.Register(new List<CurrencyPair>() { CurrencyPair.BtcEur });
            //client.TickerChanged += Client_TradeChanged;
            //client.OrderBookChanged += Client_OrderBookChanged;
            //client.Start();






            var client2 = new GdaxDataClient();
            client2.OrderBookChanged += Client_OrderBookChanged;
            client2.Register(new List<CurrencyPair>() { CurrencyPair.BtcEur });
            client2.Start();

            var task = Task.Run(() =>
            {
                while (true)
                {
                    

                    Thread.Sleep(300);
                }
            });

            Console.ReadLine();
        }

        private static void Client_OrderBookChanged(CurrencyPair arg1, OrderBook orderbook)
        {

            Console.SetCursorPosition(0, 2);

            Console.WriteLine(orderbook);
        }

        private static void Client_TradeChanged(CurrencyPair obj, Ticker ticker)
        {
            Console.WriteLine(ticker);
        }
    }
}
