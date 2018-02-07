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
            CreateMap<Observation, Observation>().ForMember(it => it.Id, config => config.Ignore());
            CreateMap<Observation, Observation>().ForMember(it => it.DateCreated, config => config.Ignore());

         
        }
    }
}
