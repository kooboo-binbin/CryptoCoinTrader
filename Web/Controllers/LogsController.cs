using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[Controller]")]
    public class LogsController : BaseController
    {
        private CoinContext _coinContext;
        public LogsController(CoinContext coinContext)
        {
            _coinContext = coinContext;
        }

        [HttpGet]
        public IActionResult Get(string observationName, string keyword, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            var query = _coinContext.Logs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(observationName))
            {
                query = query.Where(it => it.ObservationName.Contains(observationName));
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(it => it.Message.Contains(keyword));
            }
            if (startDate.HasValue)
            {
                query = query.Where(it => it.DateCreated > startDate);
            }
            if (endDate.HasValue)
            {
                query = query.Where(it => it.DateCreated < endDate);
            }
            var pageModel = new PageModel<Log>();
            Paging(query, page, pageSize, pageModel);

            return Ok(pageModel);
        }
    }
}
