using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Mappings
{
    public class LabProfile : Profile
    {
        public LabProfile()
        {
            CreateMap<Lab, LabDTO>();
            CreateMap<Lab, LabDTO>()
                .ForMember(dest => dest.labId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.labName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description));

            CreateMap<CreateLabDTO, Lab>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
           .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
           .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
           .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.managerId));

            CreateMap<UpdateLabDTO, Lab>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.mananger_Id))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.mananger_Id));

        }
    }
}
