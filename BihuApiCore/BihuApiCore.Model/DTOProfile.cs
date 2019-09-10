using AutoMapper;
using BihuApiCore.EntityFrameworkCore.Models;
using BihuApiCore.Model.Dto;
using BihuApiCore.Model.Models;

namespace BihuApiCore.Model
{
    public class DtoProfile : Profile
    {
        public DtoProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<User, UserDto>();
            CreateMap<DataExcel, ZsPiccCall>();
            //CreateMap<FlowChartViewModel, FlowChartTreeViewModel>().ForMember(dest => dest.ChildrenNodes, opt => opt.MapFrom(src => new List<FlowChartTreeViewModel>()));
            CreateMap<ModuleTreeViewModel, ManagerModuleViewModel>()
                .ForMember(dest => dest.module_code, opt => opt.MapFrom(src => src.ModuleCode))
                .ForMember(dest => dest.text, opt => opt.MapFrom(src => src.ModuleName))
                .ForMember(dest => dest.pater_code, opt => opt.MapFrom(src => src.ParentCode))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.action_url, opt => opt.MapFrom(src => src.ActionUrl))
                .ForMember(dest => dest.orderby, opt => opt.MapFrom(src => src.OrderBy))
                .ForMember(dest => dest.nodes, opt => opt.MapFrom(src => src.Nodes))
                .ForMember(dest => dest.attrs, opt => opt.MapFrom(src => src.Nodes))
                ;



        }

    }
}
