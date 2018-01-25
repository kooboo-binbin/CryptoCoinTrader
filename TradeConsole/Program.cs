using System;
using System.Collections.Generic;
using CryptoCoinTrader.Core.Exchanges;
using CryptoCoinTrader.Manifest.Enums;

namespace TradeConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var client = new BitStampDataClient();
            client.Register(new List<CurrencyPair>() { CurrencyPair.BtcEur });
            client.Start();
            Console.ReadLine();
        }
    }
}
