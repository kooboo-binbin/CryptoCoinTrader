using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Exchanges.Bl3p.Infos;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebSocket4Net;

namespace CryptoCoinTrader.Core.Exchanges.Bl3p
{
    public class Bl3pDataClient : IBl3pDataClient
    {
        public string Name => Constants.Name;

        public DateTime DateLastUpdated => DateTime.UtcNow;
        private List<CurrencyPair> _currencyPairs;
        private Dictionary<CurrencyPair, Ticker> _tickerDict = new Dictionary<CurrencyPair, Ticker>();
        private Dictionary<CurrencyPair, OrderBook> _orderBookDict = new Dictionary<CurrencyPair, OrderBook>();
        private readonly IBl3pCurrencyMapper _bl3pCurrencyMapper;
        private readonly ILogger<Bl3pDataClient> _logger;

        public Bl3pDataClient(IBl3pCurrencyMapper bl3PCurrencyMapper,
            ILogger<Bl3pDataClient> logger)
        {
            _bl3pCurrencyMapper = bl3PCurrencyMapper;
            _logger = logger;

        }

        /// <summary>
        /// Bl3p only support BTCEUR LTCEUR
        /// </summary>
        /// <param name="pairs"></param>
        public void Register(List<CurrencyPair> pairs)
        {
            _currencyPairs = pairs;
            foreach (var pair in pairs)
            {
                _tickerDict[pair] = new Ticker();
                _orderBookDict[pair] = new OrderBook()
                {
                    CurrencyPair = pair,
                    Bids = new List<OrderBookItem>(),
                    Asks = new List<OrderBookItem>()
                };
            }
        }

        public OrderBook GetOrderBook(CurrencyPair pair)
        {
            return _orderBookDict[pair];
        }

        public void Start()
        {
            foreach (var item in _currencyPairs)
            {
                var pairName = _bl3pCurrencyMapper.GetPairName(item);
                var url = $"wss://api.bl3p.eu/1/{pairName}/orderbook";
                Start(url);
            }
        }

        private void Start(string url)
        {
            var socket = new Bl3pWebSocket(url);
            socket.Opened += Socket_Opened;
            socket.MessageReceived += Socket_MessageReceived;
            socket.Closed += Socket_Closed;
            socket.Open();
        }

        private void Socket_Closed(object sender, EventArgs e)
        {
            var socket = sender as Bl3pWebSocket;

            Start(socket.Url);
        }

        private void Socket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Bl3p Opened");
        }

        private void Socket_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            try
            {
                var bl3pBook = JsonConvert.DeserializeObject<Bl3pOrderBook>(e.Message);
                var currencyPair = _bl3pCurrencyMapper.GetCurrency(bl3pBook.MarketPlace);

                var book = new OrderBook();
                book.Asks = new List<OrderBookItem>();
                book.Bids = new List<OrderBookItem>();
                book.CurrencyPair = currencyPair;

                foreach (var item in bl3pBook.Bids)
                {
                    book.Bids.Add(new OrderBookItem() { Price = item.Price / 100000m, Volume = item.Amount / 100000000m });
                }
                foreach (var item in bl3pBook.Asks)
                {
                    book.Asks.Add(new OrderBookItem() { Price = item.Price / 100000m, Volume = item.Amount / 100000000m }); //Amount / 1e8
                }
                _orderBookDict[currencyPair] = book;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Bl3p data client get a wrong message");
            }

        }


    }
}
