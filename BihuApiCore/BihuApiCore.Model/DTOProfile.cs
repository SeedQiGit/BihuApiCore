using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BihuApiCore.Model
{
    public class DTOProfile : Profile
    {
        public DTOProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<User, UserDto>();
            CreateMap<DataExcel, ZsPiccCall>();
            //CreateMap<FlowChartViewModel, FlowChartTreeViewModel>().ForMember(dest => dest.ChildrenNodes, opt => opt.MapFrom(src => new List<FlowChartTreeViewModel>()));
        }

    }
}
