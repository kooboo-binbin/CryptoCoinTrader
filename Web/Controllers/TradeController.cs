using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Trades;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class TradeController : Controller
    {
        private readonly IWorker _worker;
        private readonly ISelfInspectionService _insepectionService;
        private readonly IExchangeTradeService _exchangeTradeService;
        public TradeController(IWorker worker,
            ISelfInspectionService inspectionService,
            IExchangeTradeService exchangeTradeService)
        {
            _worker = worker;
            _insepectionService = inspectionService;
            _exchangeTradeService = exchangeTradeService;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            var model = new StatusModel();
            model.Running = _worker.GetStatus();
            return Ok(model);
        }

        [HttpPut]
        public IActionResult UpdateStatus([FromBody]StatusModel model)
        {
            MethodResult result = new MethodResult() { IsSuccessful = true };
            if (model.Running)
            {
                result = _insepectionService.Inspect();
                if (result.IsSuccessful)
                {
                    _worker.Start();
                }
            }
            else
            {
                _worker.Stop();
            }
            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult Test([FromBody] TradeTestModel model)
        {
            var orderRequest = new OrderRequest();
            orderRequest.ClientOrderId = Guid.NewGuid().ToString();
            orderRequest.CurrencyPair = model.CurrencyPair;
            orderRequest.OrderType = OrderType.Market;
            //orderRequest.Price
            orderRequest.TradeType = model.TradeType;
            orderRequest.Volume = model.Volume;
            var orderResult = _exchangeTradeService.MakeANewOrder(model.ExchangeName, orderRequest);
            return Ok(orderResult);
        }
    }
}
