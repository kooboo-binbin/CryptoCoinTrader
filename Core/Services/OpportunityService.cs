using System;
using System.Collections.Generic;
using System.Text;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services.Arbitrages;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Core.Services.Orders;
using CryptoCoinTrader.Manifest.Enums;
using Microsoft.Extensions.Logging;

namespace CryptoCoinTrader.Core.Services
{
    public class OpportunityService : IOpportunityService
    {
        private readonly IArbitrageService _arbitrageService;
        private readonly IOrderService _orderService;
        private readonly IExchangeTradeService _exchangeTradeService;
        private readonly ILogger<OpportunityService> _logger;

        public OpportunityService(IArbitrageService arbitrageService,
            IOrderService orderService,
            IExchangeTradeService exchangeTradeService,
            ILogger<OpportunityService> logger)
        {
            _arbitrageService = arbitrageService;
            _orderService = orderService;
            _exchangeTradeService = exchangeTradeService;
            _logger = logger;
        }

        public bool CheckCurrentPrice(Observation observation, decimal askPrice, decimal spreadValue, decimal spreadVolume)
        {
            if (spreadVolume < observation.MinimumVolume)
            {
                return false;
            }
            var canArbitrage = false;
            if (observation.SpreadType == SpreadType.Percentage)
            {
                if ((spreadValue / askPrice) > observation.SpreadPercentage)
                {
                    canArbitrage = true;
                }
            }
            else
            {
                if (spreadValue > observation.SpreadValue)
                {
                    canArbitrage = true;
                }
            }

            return canArbitrage;
        }

        public bool CheckLastArbitrage(Guid observationId)
        {
            var arbitrage = _arbitrageService.GetLastOne(observationId);
            if (arbitrage == null)
            {
                return true;
            }
            var orders = _orderService.GetList(arbitrage.Id);
            if (orders.Count == 0)
            {
                return true;
            }
            else
            {
                if (orders.Count == 2)
                {
                    foreach (var item in orders)
                    {
                        if (item.OrderStatus == OrderStatus.Finished)
                        {
                            continue;
                        }
                        var status = _exchangeTradeService.GetOrderStatus(item.ExchangeName, item.RemoteId);
                        if (status.Data != OrderStatus.Finished)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    _logger.LogCritical($"observiation {observationId} just have 1 order");
                    return false;
                }
            }
        }
    }
}
