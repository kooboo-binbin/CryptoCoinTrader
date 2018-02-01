using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Helpers;
using CryptoCoinTrader.Manifest.Infos;
using CryptoCoinTrader.Manifest.Interfaces;
using PusherClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CryptoCoinTrader.Core.Exchanges.Bitstamp
{
    public class BitstampDataClient : IBitstampDataClient
    {
        private readonly IBitstampCurrencyMapper _currencyMapper;
        private DateTime _dateLastUpdated = DateTime.UtcNow;
        private List<CurrencyPair> _currencyPairs;
        private Dictionary<CurrencyPair, Ticker> _tickerDict = new Dictionary<CurrencyPair, Ticker>();
        private Dictionary<CurrencyPair, OrderBook> _orderBookDict = new Dictionary<CurrencyPair, OrderBook>();

        public event Action<CurrencyPair, Ticker> TickerChanged;
        public event Action<CurrencyPair, OrderBook> OrderBookChanged;

        public BitstampDataClient(IBitstampCurrencyMapper currencyMapper)
        {
            _currencyMapper = currencyMapper;
        }

        public string Name
        {
            get { return "Bitstamp"; }
        }

        public DateTime DateLastUpdated
        {
            get { return _dateLastUpdated; }
        }


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

        public void Start()
        {
            var pusher = new PusherClient.Pusher("de504dc5763aeef9ff52");
            pusher.ConnectionStateChanged += _pusher_ConnectionStateChanged;
            pusher.Error += _pusher_Error;
            foreach (var pair in _currencyPairs)
            {
                // Setup private channel
                // RegisterTradeChannel(pusher, pair);
                RegisterOrderBookChannel(pusher, pair);
            }
            pusher.Connect();
        }

        public OrderBook GetOrderBook(CurrencyPair pair)
        {
            return _orderBookDict[pair];
        }

        private void RegisterTradeChannel(Pusher pusher, CurrencyPair pair)
        {
            var tradeChannel = pusher.Subscribe($"live_trades{GetSubscriptionName(pair)}");
            tradeChannel.Subscribed += _chatChannel_Subscribed;

            tradeChannel.Bind("trade", (dynamic data) =>
            {
                var ticker = _tickerDict[pair];
                ticker.CurrencyPair = pair;
                ticker.DateTime = TimeHelper.GetTime((long)data.timestamp);
                ticker.Id = data.id;
                ticker.Volume = data.amount;
                ticker.Price = data.price;
                ticker.TradeType = (data.type == 0);
                TickerChanged?.Invoke(pair, ticker);
            });
        }

        private void RegisterOrderBookChannel(Pusher pusher, CurrencyPair pair)
        {
            var orderBookChannel = pusher.Subscribe($"order_book{GetSubscriptionName(pair)}");
            orderBookChannel.Subscribed += _chatChannel_Subscribed;

            orderBookChannel.Bind("data", (dynamic data) =>
            {
                _dateLastUpdated = DateTime.UtcNow;
                var orderBook = new OrderBook();
                orderBook.CurrencyPair = pair;
                foreach (var bid in data.bids)
                {
                    var orderBookItem = new OrderBookItem
                    {
                        Volume = bid[1],
                        Price = bid[0]
                    };
                    orderBook.Bids.Add(orderBookItem);
                }
                foreach (var ask in data.asks)
                {
                    var orderBookItem = new OrderBookItem
                    {
                        Volume = ask[1],
                        Price = ask[0]
                    };
                    orderBook.Asks.Add(orderBookItem);
                }
                _orderBookDict[pair] = orderBook;
                OrderBookChanged?.Invoke(pair, orderBook);
            });
        }

        public Ticker GetTicker(CurrencyPair pair)
        {
            return _tickerDict[pair];
        }

        private void _chatChannel_Subscribed(object sender)
        {

        }

        static void _pusher_Error(object sender, PusherException error)
        {
            Console.WriteLine("Pusher Error: " + error.ToString());
        }

        static void _pusher_ConnectionStateChanged(object sender, ConnectionState state)
        {
            Console.WriteLine("Connection state: " + state.ToString());
        }

        private string GetSubscriptionName(CurrencyPair pair)
        {
            if (pair == CurrencyPair.BtcUsd)
            {
                return "";
            }
            return $"_{_currencyMapper.GetPairName(pair)}";
        }
    }
}
