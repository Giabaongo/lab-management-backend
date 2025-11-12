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
            CreateMap<Lab, LabDTO>()
                .ForMember(dest => dest.labId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.labName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.managerId, opt => opt.MapFrom(src => src.ManagerId))
                .ForMember(dest => dest.location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.departmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.departmentName, opt => opt.MapFrom(src => src.Department.Name))
                .ForMember(dest => dest.isPublic, opt => opt.MapFrom(src => src.Department.IsPublic));

            CreateMap<CreateLabDTO, Lab>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.managerId))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.departmentId));

            CreateMap<UpdateLabDTO, Lab>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name))
                .ForMember(dest => dest.ManagerId, opt => opt.MapFrom(src => src.mananger_Id))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.location))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.description))
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.departmentId));
        }
    }
}
