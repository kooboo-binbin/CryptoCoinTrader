using AutoMapper;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Core.Services;
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
        private readonly IMapper _mapper;
        private readonly IObservationService _observationService;
        public ObservationsController(IMapper mapper,
            IObservationService observationService)
        {
            _observationService = observationService;
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
            return NoContent();
        }

    }
}
