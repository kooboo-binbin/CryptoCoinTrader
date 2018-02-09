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

        private static Dictionary<Guid, CancellationTokenSource> _tasks = new Dictionary<Guid, CancellationTokenSource>();

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
            foreach (var item in observations)
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
            _tasks.Add(observation.Id, tokenSource);
        }

        public void Delete(Guid obvervationId)
        {
            if (_tasks.ContainsKey(obvervationId))
            {
                var tokenSource = _tasks[obvervationId];
                tokenSource.Cancel();
                _tasks.Remove(obvervationId);
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
                    var bookBuy = _exchangeDataService.GetOrderBook(observation.BuyExchangeName, observation.CurrencyPair);
                    var bookSell = _exchangeDataService.GetOrderBook(observation.SellExchangeName, observation.CurrencyPair);

                    if (bookBuy.Bids.Count > 0 && bookSell.Asks.Count > 0)
                    {
                        var buy_ask_0 = bookBuy.Asks[0]; //the sell price of the exchange which we want to buy.
                        var sell_bid_0 = bookSell.Bids[0]; //the buy price of the exchange which we wat to sell.
                        var spread = sell_bid_0.Price - buy_ask_0.Price; //some one buy price is greater than the price some one want to sell. then we have a chance to make a arbitrage
                        var spreadVolume = Math.Min(sell_bid_0.Volume, buy_ask_0.Volume);

                        var canArbitrage = _opportunityService.CheckCurrentPrice(observation, buy_ask_0.Price, spread, spreadVolume);
                        var lastArbitrage = _opportunityService.CheckLastArbitrage(observation.Id);
                        if (canArbitrage & lastArbitrage)
                        {
                            DoArbitrage(observation, spreadVolume);
                        }
                    }
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    _messageService.Error(observation.Id, "Get an unhandled exception");
                    observation.RunningStatus = RunningStatus.Error;
                    _logger.LogCritical(ex, "RunObservation failed.");
                }
            }
        }


        private void DoArbitrage(Observation observation, decimal spreadVolume)
        {
            var volume = Math.Min(observation.PerVolume, observation.AvailabeVolume);
            volume = Math.Min(volume, spreadVolume);

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
            var buyResult = _exchangeTradeService.MakeANewOrder(observation.BuyExchangeName, buyRequest);
            var sellResult = _exchangeTradeService.MakeANewOrder(observation.SellExchangeName, sellRequest);
            if (!buyResult.IsSuccessful)
            {
                var message = $"Make a buy failed {buyResult.Message} {observation.GetName()}";
                _messageService.Error(observation.Id, message);
                observation.RunningStatus = RunningStatus.Error;
                _logger.LogError(message);
            }
            if (!sellResult.IsSuccessful)
            {
                var message = $"Make a sell order failed {sellResult.Message} {observation.GetName()}";
                _messageService.Error(observation.Id, message);
                observation.RunningStatus = RunningStatus.Error;
                _logger.LogError(message);
            }
            if (buyResult.IsSuccessful ^ sellResult.IsSuccessful)
            {
                var message = $"only one order is executed!!!! buy {buyResult.IsSuccessful} sell {sellResult.IsSuccessful}  {observation.GetName()}";
                _messageService.Error(observation.Id, message);
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
                    Volume = volume
                };
                _arbitrageService.Add(arbitrage);

                var buyOrder = new Order
                {
                    ArbitrageId = arbitrage.Id,
                    DateCreated = DateTime.UtcNow,
                    ExchangeName = observation.BuyExchangeName,
                    Id = Guid.NewGuid(),
                    OrderStatus = buyResult.Data.Status,
                    Price = 0,//it is market,
                    RemoteId = buyResult.Data.Id,
                    Target = observation.CurrencyPair,
                    Volume = volume
                };
                var sellOrder = new Order
                {
                    ArbitrageId = arbitrage.Id,
                    DateCreated = DateTime.UtcNow,
                    ExchangeName = observation.SellExchangeName,
                    Id = Guid.NewGuid(),
                    OrderStatus = buyResult.Data.Status,
                    Price = 0,//it is market,
                    RemoteId = buyResult.Data.Id,
                    Target = observation.CurrencyPair,
                    Volume = volume
                };
                _orderService.Add(buyOrder, sellOrder);
                _messageService.Write(observation.Id, $"{observation.GetName()} do a arbitrage");
            }

        }
    }
}
