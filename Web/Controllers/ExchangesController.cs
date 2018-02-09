using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Services.Exchanges;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Interfaces;
using CryptoCoinTrader.Web.Models;
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
        private readonly IExchangeSetting _exchangeSetting;
        private readonly IExchangeConfigService _exchangeConfigService;


        public ExchangesController(IExchangeDataService exchangeDataService,
            IExchangeSetting exchangeSetting,
            IExchangeConfigService exchangeConfigService)
        {
            _exchangeDataService = exchangeDataService;
            _exchangeSetting = exchangeSetting;

            _exchangeConfigService = exchangeConfigService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var names = _exchangeDataService.GetExchangeNames();
            return Ok(names);
        }

        [HttpGet("[action]")]
        public IActionResult Settings()
        {
            _exchangeConfigService.GetConfigs();
            var exchanges = new List<ExchangeModel>();
            var names = _exchangeDataService.GetExchangeNames();
            foreach (var item in names)
            {
                var model = new ExchangeModel
                {
                    Name = item,
                    Settings = _exchangeSetting.GetSetting(item)
                };
                exchanges.Add(model);
            }
            return Ok(exchanges);
        }

        [HttpPut("[action]")]
        public IActionResult Settings([FromBody]ExchangeModel model)
        {
            var result = _exchangeConfigService.Validate(model.Name, model.Settings);
            if (!result.IsSuccessful)
            {
                return Ok(result);
            }
            _exchangeSetting.SaveSetting(model.Name, model.Settings);
            result = new MethodResult() { IsSuccessful = true };
            return Ok(result);
        }

    }
}
