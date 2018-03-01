using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Arbitrages;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Core.Services.Orders;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Manifest.Trades;
using Microsoft.Extensions.Logging;

namespace CryptoCoinTrader.Core.Workers
{
    public class Worker : IWorker
    {
        private readonly IExchangeDataService _exchangeDataService;
        private readonly IExchangeTradeService _exchangeTradeService;
        private readonly IMessageService _messageService;
        private readonly IObservationService _observationService;
        private readonly IOpportunityService _opportunityService;
        private readonly IArbitrageService _arbitrageService;
        private readonly IOrderService _orderService;
        private readonly ILogger<Worker> _logger;

        private static Dictionary<Guid, CancellationTokenSource> _taskDict = new Dictionary<Guid, CancellationTokenSource>();
        private static List<Task> _tasks = new List<Task>();
        private static bool _running { get; set; }

        public Worker(IExchangeDataService exchangeDataService,
            IExchangeTradeService exchangeTradeService,
            IMessageService messageService,
            IObservationService observationService,
            IOpportunityService opportunityService,
            IArbitrageService arbitrageService,
            IOrderService orderService,
            ILogger<Worker> logger)
        {
            _exchangeDataService = exchangeDataService;
            _exchangeTradeService = exchangeTradeService;
            _messageService = messageService;
            _observationService = observationService;
            _opportunityService = opportunityService;
            _arbitrageService = arbitrageService;
            _orderService = orderService;
            _logger = logger;
        }

        public void Add(List<Observation> observations)
        {
            foreach (var item in observations.ToArray())
            {
                Add(item);
            }
        }

        public void Add(Observation observation)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var task = Task.Run(() =>
            {
                RunObservatoin(observation, token);
            }, token);
            _tasks.Add(task);
            _taskDict.Add(observation.Id, tokenSource);
        }

        public void Delete(Guid obvervationId)
        {
            if (_taskDict.ContainsKey(obvervationId))
            {
                var tokenSource = _taskDict[obvervationId];
                tokenSource.Cancel();
                _taskDict.Remove(obvervationId);
            }
        }

        public void Start()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        public bool GetStatus()
        {
            return _running;
        }

        private void RunObservatoin(Observation observation, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (!_running || observation.RunningStatus != RunningStatus.Running)
                {
                    Thread.Sleep(2 * 1000);
                    continue;
                }
                try
                {
                    var bookBuy = _exchangeDataService.GetOrderBook(observation.FromExchangeName, observation.CurrencyPair);
                    var bookSell = _exchangeDataService.GetOrderBook(observation.ToExchangeName, observation.CurrencyPair);
                    var timeFrom = _exchangeDataService.GetLastUpdated(observation.FromExchangeName);
                    var timeTo = _exchangeDataService.GetLastUpdated(observation.ToExchangeName);
                    var ts = Math.Abs((timeFrom - timeTo).TotalSeconds);
                    var ts2 = Math.Abs((DateTime.UtcNow - timeFrom).TotalSeconds);
                    if (Math.Max(ts, ts2) > 10)
                    {
                        _logger.LogError("Date is not updated");
                        continue;// the data has no be udpated recently;
                    }
                    if (bookBuy.Bids.Count > 0 && bookSell.Asks.Count > 0)
                    {

                        var buy_ask_0 = bookBuy.Asks[0]; //the sell price of the exchange which we want to buy.
                        var sell_bid_0 = bookSell.Bids[0]; //the buy price of the exchange which we wat to sell.
                        var spreadValue = sell_bid_0.Price - buy_ask_0.Price; //some one buy price is greater than the price some one want to sell. then we have a chance to make a arbitrage
                        var spreadVolume = Math.Min(sell_bid_0.Volume, buy_ask_0.Volume);

                        var info = new ArbitrageInfo
                        {
                            SpreadValue = spreadValue,
                            SpreadVolume = spreadVolume,
                            FromPrice = buy_ask_0.Price,
                            ToPrice = sell_bid_0.Price,
                        };

                        var canArbitrage = _opportunityService.CheckCurrentPrice(observation, info);
                        var lastArbitrage = _opportunityService.CheckLastArbitrage(observation.Id);
                        if (canArbitrage & lastArbitrage)
                        {

                            DoArbitrage(observation, info);
                        }
                    }
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    _messageService.Error(observation.Id, observation.Name, "Get an unhandled exception" + ex.ToString());
                    observation.RunningStatus = RunningStatus.Error;
                    _observationService.Update(observation);
                    _logger.LogCritical(ex, "RunObservation failed.");
                }
            }
        }


        private void DoArbitrage(Observation observation, ArbitrageInfo info)
        {
            var volume = Math.Min(observation.PerVolume, observation.AvailabeVolume);
            volume = Math.Min(volume, info.SpreadVolume);

            var orderBuyId = Guid.NewGuid();
            var orderSellId = Guid.NewGuid();
            var buyRequest = new OrderRequest()
            {
                ClientOrderId = orderBuyId.ToString(),
                CurrencyPair = observation.CurrencyPair,
                OrderType = OrderType.Market,
                Price = 0m,
                TradeType = TradeType.Buy,
                Volume = volume,
            };
            var sellRequest = new OrderRequest()
            {
                ClientOrderId = orderSellId.ToString(),
                CurrencyPair = observation.CurrencyPair,
                OrderType = OrderType.Market,
                Price = 0m,
                TradeType = TradeType.Sell,
                Volume = volume,
            };
            var buyResult = _exchangeTradeService.MakeANewOrder(observation.FromExchangeName, buyRequest);
            var sellResult = _exchangeTradeService.MakeANewOrder(observation.ToExchangeName, sellRequest);
            if (!buyResult.IsSuccessful)
            {
                var message = $"Make a buy failed {buyResult.Message} {observation.GetName()}";
                _messageService.Error(observation.Id, observation.Name, message);
                observation.RunningStatus = RunningStatus.Error;
                _logger.LogError(message);
            }
            if (!sellResult.IsSuccessful)
            {
                var message = $"Make a sell order failed {sellResult.Message} {observation.GetName()}";
                _messageService.Error(observation.Id, observation.Name, message);
                observation.RunningStatus = RunningStatus.Error;
                _logger.LogError(message);
            }
            if (buyResult.IsSuccessful ^ sellResult.IsSuccessful)
            {
                var message = $"only one order is executed!!!! buy {buyResult.IsSuccessful} sell {sellResult.IsSuccessful}  {observation.GetName()}";
                _messageService.Error(observation.Id, observation.Name, message);
                _logger.LogCritical(message);
            }
            if (buyResult.IsSuccessful && sellResult.IsSuccessful)
            {
                _observationService.SubtractAvailabeVolume(observation.Id, volume);

                var arbitrage = new Arbitrage
                {
                    DateCreated = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    ObservationId = observation.Id,
                    ObservationName = observation.Name,
                    Volume = volume,
                    Spread = info.SpreadValue
                };
                _arbitrageService.Add(arbitrage);

                var buyOrder = new Order
                {
                    ArbitrageId = arbitrage.Id,
                    ObservationId = arbitrage.ObservationId,
                    ObservationName = arbitrage.ObservationName,
                    DateCreated = DateTime.UtcNow,
                    ExchangeName = observation.FromExchangeName,
                    Id = Guid.NewGuid(),
                    OrderStatus = buyResult.Data.Status,
                    Price = info.FromPrice,
                    RemoteId = buyResult.Data.Id,
                    CurrencyPair = observation.CurrencyPair,
                    TradeType = TradeType.Buy,
                    Volume = volume,
                };
                var sellOrder = new Order
                {
                    ArbitrageId = arbitrage.Id,
                    ObservationId = arbitrage.ObservationId,
                    ObservationName = arbitrage.ObservationName,
                    DateCreated = DateTime.UtcNow,
                    ExchangeName = observation.ToExchangeName,
                    Id = Guid.NewGuid(),
                    OrderStatus = buyResult.Data.Status,
                    Price = info.ToPrice,
                    RemoteId = buyResult.Data.Id,
                    CurrencyPair = observation.CurrencyPair,
                    TradeType = TradeType.Sell,
                    Volume = volume
                };
                _orderService.Add(buyOrder, sellOrder);
                _messageService.Write(observation.Id, observation.Name, $"{observation.Name} do a arbitrage {DateTime.UtcNow}");
            }

        }


    }
}
