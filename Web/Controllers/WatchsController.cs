using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Manifest.Infos;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class WatchsController : BaseController
    {
        private IObservationService _observationService;
        private IExchangeDataService _exchangeDataService;
        public WatchsController(IObservationService observationService,
            IExchangeDataService exchangeDataService)
        {
            _observationService = observationService;
            _exchangeDataService = exchangeDataService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var observations = _observationService.GetObservations();
            var list = new List<WatchModel>();
            foreach (var item in observations)
            {
                var orderBook = new WatchOrderBookModel();

                var sell_book = _exchangeDataService.GetOrderBook(item.ToExchangeName, item.CurrencyPair);
                var buy_book = _exchangeDataService.GetOrderBook(item.FromExchangeName, item.CurrencyPair);
                var bid1 = new OrderBookItem();
                var ask1 = new OrderBookItem();
                if (sell_book.Bids.Count > 0)
                {
                    bid1 = sell_book.Bids[0];
                    orderBook.Bid1 = bid1.ToString();
                    orderBook.Bid2 = sell_book.Bids[1].ToString();
                    orderBook.Bid3 = sell_book.Bids[2].ToString();
                }
                else
                {
                    orderBook.Bid1 = "";
                    orderBook.Bid2 = "";
                    orderBook.Bid3 = "";
                }
                if (buy_book.Asks.Count > 0)
                {
                    ask1 = buy_book.Asks[0];
                    orderBook.Ask1 = ask1.ToString();
                    orderBook.Ask2 = buy_book.Asks[1].ToString();
                    orderBook.Ask3 = buy_book.Asks[2].ToString();
                }
                else
                {
                    orderBook.Ask1 = "";
                    orderBook.Ask2 = "";
                    orderBook.Ask3 = "";
                }
                orderBook.SpreadValue = (bid1.Price - ask1.Price).ToString("f2");
                orderBook.SpreadVolume = Math.Min(bid1.Volume, ask1.Volume).ToString("f4");

                var model = new WatchModel()
                {
                    Observation = item,
                    OrderBook = orderBook
                };
                list.Add(model);
            }
            return Ok(list);
        }
    }
}
