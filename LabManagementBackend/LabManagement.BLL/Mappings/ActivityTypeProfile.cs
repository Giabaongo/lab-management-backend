using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings
{
    public class ActivityTypeProfile : Profile
    {
        public ActivityTypeProfile()
        {
            CreateMap<ActivityType, ActivityTypeDTO>();
            CreateMap<CreateActivityTypeDTO, ActivityType>();
            CreateMap<UpdateActivityTypeDTO, ActivityType>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
