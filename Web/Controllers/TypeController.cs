using CryptoCoinTrader.Manifest.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class TypeController : Controller
    {
        [HttpGet("[action]")]
        public IActionResult RunningStatus()
        {
            var names = Enum.GetNames(typeof(RunningStatus));

            return Ok(names);
        }
    }
}
