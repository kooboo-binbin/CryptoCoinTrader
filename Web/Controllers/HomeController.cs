using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoCoinTrader.Core.Data;
using CryptoCoinTrader.Core.Data.Entities;
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
            var kk = _context.Arbitrages.ToList();
            var temp = new Arbitrage();
            temp.Id = Guid.NewGuid();
            
            _context.Add(temp);
            _context.SaveChanges();
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
