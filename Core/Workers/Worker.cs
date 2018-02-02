using System;
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

        public void Work(List<Observation> observations)
        {
            for (int i = 0; i < observations.Count; i++)
            {
                var observation = observations[i];
                var task = Task.Run(() =>
                {
                    RunObservatoin(i, observation);
                });
            }
        }

        private void RunObservatoin(int index, Observation observation)
        {
            while (observation.RunningStatus == RunningStatus.Running)
            {
                try
                {
                    var top = index * 5;
                    _messageService.Write(top + 0, observation.ToConsole());
                    var book1 = _exchangeDataService.GetOrderBook(observation.BuyExchangeName, observation.CurrencyPair);
                    var book2 = _exchangeDataService.GetOrderBook(observation.SellExchangeName, observation.CurrencyPair);

                    if (book1.Bids.Count > 0 && book2.Asks.Count > 0)
                    {
                        var ask1_0 = book1.Asks[0];
                        var bid2_0 = book2.Bids[0];
                        var spread = bid2_0.Price - ask1_0.Price;
                        var spreadVolume = Math.Min(bid2_0.Volume, ask1_0.Volume);
                        var spreadMessage = $"Spread {observation.SellExchangeName}.bid1 {bid2_0.Price:f2} - {observation.BuyExchangeName}.ask1 {ask1_0.Price:f2} = {spread:f2} volume:{spreadVolume}";
                        _messageService.Write(top + 1, spreadMessage);

                        var canArbitrage = _opportunityService.CheckCurrentPrice(observation, ask1_0.Price, spread);
                        var lastArbitrage = _opportunityService.CheckLastArbitrage(observation.Id);
                        if (canArbitrage & lastArbitrage)
                        {
                            DoArbitrage(top, observation, spreadVolume);
                        }
                        LastArbitrageMessage(top + 3, canArbitrage, lastArbitrage);
                    }
                    Thread.Sleep(200);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "RunObservation failed.");
                }
            }
        }

        private void DoArbitrage(int top, Observation observation, decimal spreadVolume)
        {
            var volume = Math.Min(observation.PerVolume, observation.AvaialbeVolume);
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
                observation.RunningStatus = RunningStatus.Error;
                _messageService.Write(top + 0, observation.ToConsole());
                _logger.LogError($"Make a buy order failed {buyResult.Message}");
            }
            if (!buyResult.IsSuccessful)
            {
                observation.RunningStatus = RunningStatus.Error;
                _messageService.Write(top + 0, observation.ToConsole());
                _logger.LogError($"Make a sell order failed {sellResult.Message}");
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
            }

        }

        private void LastArbitrageMessage(int top, bool canArbitrage, bool lastArbitrage)
        {
            if (canArbitrage & !lastArbitrage)
            {
                _messageService.Write(top, "Last arbitrage is not finished.");
            }
            else
            {
                _messageService.Write(top, "                                                                                    ");
            }
        }
    }
}
