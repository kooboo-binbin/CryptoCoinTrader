using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services.Orders;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[Controller]")]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Get(Guid? arbitrageId, string observationName, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 20)
        {
            var query = _orderService.GetQuery();
            if (arbitrageId.HasValue)
            {
                query = query.Where(it => it.ArbitrageId == arbitrageId);
            }
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
            var pageModel = new PageModel<Order>();
            Paging(query, page, pageSize, pageModel);
            return Ok(pageModel);
        }
    }
}
