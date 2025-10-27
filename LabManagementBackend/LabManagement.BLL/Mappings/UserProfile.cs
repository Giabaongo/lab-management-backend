    using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            //Get all, get by id    
            CreateMap<User, UserDTO>();

            CreateMap<CreateUserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<UpdateUserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
