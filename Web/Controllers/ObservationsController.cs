using AutoMapper;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services;
using CryptoCoinTrader.Core.Workers;
using CryptoCoinTrader.Manifest;
using CryptoCoinTrader.Manifest.Enums;
using CryptoCoinTrader.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.Controllers
{
    [Route("api/[controller]")]
    public class ObservationsController : Controller
    {

        private readonly IObservationService _observationService;
        private readonly IWorker _worker;
        public ObservationsController(IObservationService observationService,
            IWorker worker)
        {
            _observationService = observationService;
            _worker = worker;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = _observationService.GetObservations().OrderBy(it => it.DateCreated).ToList();
            return Ok(list);
        }

        [HttpPost]
        public IActionResult Post([FromBody]Observation model)
        {
            model.Id = Guid.NewGuid();
            model.DateCreated = DateTime.UtcNow;
            _observationService.Add(model);
            _worker.Add(model);
            return NoContent();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Observation model)
        {
            _observationService.Update(model);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody]RunningStatusModel model)
        {
            var observations = _observationService.GetObservations();
            var old = observations.FirstOrDefault(it => it.Id == id);
            old.RunningStatus = model.Status;
            _observationService.Update(old);
            return Ok(new MethodResult() { IsSuccessful = true, Message = "Update status successfully." });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _observationService.Delete(id);
            _worker.Delete(id);
            return NoContent();
        }

    }
}
