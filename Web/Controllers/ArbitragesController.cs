using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services.Arbitrages;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class ArbitragesController : BaseController
    {
        private readonly IArbitrageService _arbitrageService;

        public ArbitragesController(IArbitrageService arbitrageService)
        {
            _arbitrageService = arbitrageService;
        }

        [HttpGet]
        public IActionResult Get(string observationName, DateTime? startDate, DateTime? endDate, int page = 0, int pageSize = 20)
        {
            var query = _arbitrageService.GetQuery().OrderByDescending(it=>it.DateCreated).AsQueryable();
            if (!string.IsNullOrWhiteSpace(observationName))
            {
                query = query.Where(it => it.ObservationName.Contains(observationName));
            }
            if (startDate.HasValue)
            {
                query = query.Where(it => it.DateCreated > startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(it => it.DateCreated > endDate);
            }
            var pageModel = new PageModel<Arbitrage>();
            Paging(query, page, pageSize, pageModel);
            return Ok(pageModel);
        }
    }
}
