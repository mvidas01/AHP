using AutoMapper;
using AHP.Enitities;
using AHP.Models;

using System;
using System.Collections.Generic;
using System.Text;

namespace AHP.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CriterionData, CriterionIntermediateResponce>();
            CreateMap<AlternativeData, AlternativeIntermediateResponce>();
        }
    }
    public class AutoMapperConfiguration
    {
        public MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg => 
            { 
                cfg.AddProfile<AutoMapperProfile>();  
            });
            return config;
        }

    }

}
