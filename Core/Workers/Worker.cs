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
                var index = i;
                var observation = observations[i];
                var task = Task.Run(() =>
                {
                    RunObservatoin(index, observation);
                });
            }
        }

        private void RunObservatoin(int index, Observation observation)
        {
            while (observation.RunningStatus == RunningStatus.Running)
            {
                var top = index * 5;
                try
                {
                    WriteObservation(observation, top);
                    var bookBuy = _exchangeDataService.GetOrderBook(observation.BuyExchangeName, observation.CurrencyPair);
                    var bookSell = _exchangeDataService.GetOrderBook(observation.SellExchangeName, observation.CurrencyPair);

                    if (bookBuy.Bids.Count > 0 && bookSell.Asks.Count > 0)
                    {
                        var buy_ask_0 = bookBuy.Asks[0]; //the sell price of the exchange which we want to buy.
                        var sell_bid_0 = bookSell.Bids[0]; //the buy price of the exchange which we wat to sell.
                        var spread = sell_bid_0.Price - buy_ask_0.Price; //some one buy price is greater than the price some one want to sell. then we have a chance to make a arbitrage
                        var spreadVolume = Math.Min(sell_bid_0.Volume, buy_ask_0.Volume);
                        var spreadMessage = $"Spread {observation.SellExchangeName}.bid1 {sell_bid_0.Price:f2} - {observation.BuyExchangeName}.ask1 {buy_ask_0.Price:f2} = {spread:f2} volume:{spreadVolume}";
                        _messageService.Write(top + 1, spreadMessage);

                        var canArbitrage = _opportunityService.CheckCurrentPrice(observation, buy_ask_0.Price, spread, spreadVolume);
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
                    observation.RunningStatus = RunningStatus.Error;
                    WriteObservation(observation, top);
                    _logger.LogCritical(ex, "RunObservation failed.");
                }
            }
        }

        /// <summary>
        /// Output the observatoin information to console
        /// </summary>
        /// <param name="observation"></param>
        /// <param name="top"></param>
        private void WriteObservation(Observation observation, int top)
        {
            _messageService.Write(top + 0, observation.ToConsole());
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
                WriteObservation(observation, top);
                _logger.LogError($"Make a buy order failed {buyResult.Message}");
            }
            if (!sellResult.IsSuccessful)
            {
                observation.RunningStatus = RunningStatus.Error;
                WriteObservation(observation, top);
                _logger.LogError($"Make a sell order failed {sellResult.Message}");
            }
            if (buyResult.IsSuccessful ^ sellResult.IsSuccessful)
            {
                var message = $"only one order is executed!!!! buy {buyResult.IsSuccessful} sell {sellResult.IsSuccessful}";
                _messageService.Write(23, message);
                _logger.LogCritical(message);
            }
            if (buyResult.IsSuccessful && sellResult.IsSuccessful)
            {
                _observationService.SubtractAvailabeVolume(observation.Id, volume);
                if (observation.AvaialbeVolume <= 0)
                {
                    WriteObservation(observation, top);
                }
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
