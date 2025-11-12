using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDTO>();

            CreateMap<CreateDepartmentDTO, Department>();

            CreateMap<UpdateDepartmentDTO, Department>();
        }
    }
}
