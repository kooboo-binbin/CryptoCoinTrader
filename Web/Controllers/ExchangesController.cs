using CryptoCoinTrader.Core.Services.Exchanges;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class ExchangesController : Controller
    {
        private readonly IExchangeDataService _exchangeDataService;
        public ExchangesController(IExchangeDataService exchangeDataService)
        {
            _exchangeDataService = exchangeDataService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var names = _exchangeDataService.GetExchangeNames();
            return Ok(names);
        }
    }
}
