using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings
{
    public class SecurityLogProfile : Profile
    {
        public SecurityLogProfile()
        {
            CreateMap<SecurityLog, SecurityLogDTO>();
            CreateMap<CreateSecurityLogDTO, SecurityLog>()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateSecurityLogDTO, SecurityLog>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
