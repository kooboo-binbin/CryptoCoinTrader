using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class TradeController : Controller
    {
        private readonly IWorker _worker;
        private readonly ISelfInspectionService _insepectionService;
        public TradeController(IWorker worker, ISelfInspectionService inspectionService)
        {
            _worker = worker;
            _insepectionService = inspectionService;
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            var model = new StatusModel();
            model.Running = _worker.GetStatus();
            return Ok(model);
        }

        [HttpPut]
        public IActionResult UpdateStatus([FromBody]StatusModel model)
        {
            MethodResult result = new MethodResult() { IsSuccessful = true };
            if (model.Running)
            {
                result = _insepectionService.Inspect();
                if (result.IsSuccessful)
                {
                    _worker.Start();
                }
            }
            else
            {
                _worker.Stop();
            }
            return Ok(result);
        }
    }
}
