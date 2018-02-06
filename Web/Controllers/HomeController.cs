using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Exchanges.Gdax.Infos;
using Microsoft.AspNetCore.Mvc;

namespace CryptoCoinTrader.Web.Controllers
{
    public class HomeController : Controller
    {
        private CoinContext _context;
        public HomeController(CoinContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
