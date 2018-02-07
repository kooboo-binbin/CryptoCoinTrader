using AutoMapper.Configuration;
using CryptoCoinTrader.Core.Data.Entities;
using CryptoCoinTrader.Manifest.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoCoinTrader.Web.MappingProfiles
{
    public class MappingProfile : MapperConfigurationExpression
    {
        public MappingProfile()
        {
            CreateMap<Models.Observations.ObservationPostModel, Observation>().AfterMap((m, o) =>
            {
                o.Id = Guid.NewGuid();
                o.DateCreated = DateTime.UtcNow;
                o.RunningStatus = RunningStatus.Stoped;
            });
        }
    }
}
