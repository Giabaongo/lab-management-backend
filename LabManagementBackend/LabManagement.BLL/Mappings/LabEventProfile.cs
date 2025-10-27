using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings
{
    public class LabEventProfile : Profile
    {
        public LabEventProfile()
        {
            CreateMap<LabEvent, LabEventDTO>();
            CreateMap<CreateLabEventDTO, LabEvent>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateLabEventDTO, LabEvent>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
