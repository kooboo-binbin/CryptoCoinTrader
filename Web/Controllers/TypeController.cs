using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("[controller]")]
    public class TypeController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult RunningStatus()
        {
            return Content("RunningStatus");
        }
    }
}
