using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class ArbitrageController : Controller
    {
        [HttpGet]
        public IActionResult Get(Guid? observationid, DateTime? startDate, DateTime? endDate, int pageSize, int page)
        {
            return NoContent();
        }
    }
}
