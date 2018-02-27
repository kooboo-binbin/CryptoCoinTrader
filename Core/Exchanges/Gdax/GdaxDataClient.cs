using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Infos;
using CryptoCoinTrader.Manifest.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Serialization;
using System.Threading;
using System.Globalization;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using CryptoCoinTrader.Manifest.Helpers;
using CryptoCoinTrader.Core.Exchanges.Gdax.Remotes;

namespace CryptoCoinTrader.Core.Exchanges.Gdax
{
    /// <summary>
    /// Should be singleton
    /// </summary>
    public class GdaxDataClient : IGdaxDataClient
    {
        private readonly IGdaxCurrencyMapper _currencyMapper;
        private DateTime _dateLastUpdated = DateTime.UtcNow;
        private List<CurrencyPair> _currencyPairs;
        private Dictionary<CurrencyPair, Ticker> _tickerDict = new Dictionary<CurrencyPair, Ticker>();
        private Dictionary<CurrencyPair, OrderBook> _orderBookDict = new Dictionary<CurrencyPair, OrderBook>();

        //public event Action<CurrencyPair, Ticker> TickerChanged;
        public event Action<CurrencyPair, OrderBook> OrderBookChanged;

        public GdaxDataClient(IGdaxCurrencyMapper currencyMapper)
        {
            _currencyMapper = currencyMapper;
        }

        public DateTime DateLastUpdated
        {
            get
            {
                return _dateLastUpdated;
            }
        }

        public string Name
        {
            get { return Constants.Name; }
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
            //https://docs.gdax.com/#protocol-overview
            WebSocket4Net.WebSocket socket = new WebSocket4Net.WebSocket("wss://ws-feed.gdax.com");
            socket.Opened += Socket_Opened;
            socket.MessageReceived += Socket_MessageReceived;
            socket.Open();
            socket.Closed += Socket_Closed;
            //socket.Send(Resource1.subscribe);
        }

        public OrderBook GetOrderBook(CurrencyPair pair)
        {
            if (_orderBookDict.ContainsKey(pair))
            {
                return _orderBookDict[pair];
            }
            return new OrderBook();
        }

        private void Socket_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Gdax opend");
            var subscribeInfo = new GdaxSubscribe()
            {
                Type = "subscribe",
                ProductIds = _currencyPairs.Select(it => _currencyMapper.GetPairName(it)).ToList(),
                Channels = new List<object>() { "level2", "heartbeat" }
            };
            Thread.Sleep(1000);
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var subscribe = JsonConvert.SerializeObject(subscribeInfo, serializerSettings);
            ((WebSocket4Net.WebSocket)sender).Send(subscribe);
        }

        private long snapShotCount = 0;
        private void Socket_MessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            dynamic data;
            try
            {
                data = JsonConvert.DeserializeObject<dynamic>(e.Message);
            }
            catch (Exception ex)
            {
                //Todo: log error
                return;
            }
            _dateLastUpdated = DateTime.UtcNow;
            var type = (string)data.type;
            switch (type)
            {
                case "snapshot":
                    Snapshot(data);
                    snapShotCount++;
                    break;
                case "l2update":
                    L2Update(data);
                    break;
                case "heartbeat":
                    break;
                case "subscriptions":
                    break;
                default:
                    Console.WriteLine(e.Message);
                    break;
            }
        }

        private void L2Update(dynamic data)
        {
            try
            {
                var currencyPair = _currencyMapper.GetCurrency((string)data.product_id);
                var orderBook = _orderBookDict[currencyPair];
                foreach (var change in data.changes)
                {
                    var tradeType = (string)change[0];
                    var price = DecimalHelper.Get((string)change[1]);
                    var volume = DecimalHelper.Get((string)change[2]);
                    if (tradeType == "buy")
                    {
                        var bid = orderBook.Bids.FirstOrDefault(it => it.Price == price);
                        if (volume == 0)
                        {
                            if (bid != null)
                            {
                                orderBook.Bids.Remove(bid);
                                SortBids(orderBook);
                            }
                        }
                        else
                        {
                            if (bid != null)
                            {
                                bid.Volume = volume;
                            }
                            else
                            {
                                bid = new OrderBookItem()
                                {
                                    Price = price,
                                    Volume = volume
                                };
                                orderBook.Bids.Add(bid);
                                SortBids(orderBook);
                            }
                        }
                    }
                    else
                    {
                        var ask = orderBook.Asks.FirstOrDefault(it => it.Price == price);
                        if (volume == 0)
                        {
                            if (ask != null)
                            {
                                orderBook.Asks.Remove(ask);
                                SortAsks(orderBook);
                            }
                        }
                        if (ask != null)
                        {
                            ask.Volume = volume;
                        }
                        else
                        {
                            ask = new OrderBookItem()
                            {
                                Price = price,
                                Volume = volume
                            };
                            orderBook.Asks.Add(ask);
                            SortAsks(orderBook);
                        }
                    }
                    OrderBookChanged?.Invoke(currencyPair, orderBook);
                }
            }
            catch (Exception)
            {
                //Todo:log data format error  
            }
        }

        private void Snapshot(dynamic data)
        {
            var currencyPair = _currencyMapper.GetCurrency((string)data.product_id);
            var orderBook = new OrderBook
            {
                CurrencyPair = currencyPair
            };

            foreach (var bid in data.bids)
            {
                var bookItem = new OrderBookItem()
                {

                    Price = DecimalHelper.Get((string)bid[0]),
                    Volume = DecimalHelper.Get((string)bid[1]),
                };
                orderBook.Bids.Add(bookItem);
            }
            foreach (var ask in data.asks)
            {
                var bookItem = new OrderBookItem()
                {
                    Price = DecimalHelper.Get((string)ask[0]),
                    Volume = DecimalHelper.Get((string)ask[1]),
                };
                orderBook.Asks.Add(bookItem);
            }
            SortBids(orderBook);
            SortAsks(orderBook);
            _orderBookDict[currencyPair] = orderBook;
            OrderBookChanged?.Invoke(orderBook.CurrencyPair, orderBook);
        }



        private static void SortAsks(OrderBook orderBook)
        {
            orderBook.Asks = orderBook.Asks.OrderBy(it => it.Price).ToList();
        }

        private static void SortBids(OrderBook orderBook)
        {
            orderBook.Bids = orderBook.Bids.OrderByDescending(it => it.Price).ToList();
        }

        private void Socket_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Gdax closed");
            Start();
        }

    }


}
