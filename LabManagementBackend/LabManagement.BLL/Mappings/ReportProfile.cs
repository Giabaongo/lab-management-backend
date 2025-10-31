using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        CreateMap<Report, ReportDTO>()
            .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Lab != null ? src.Lab.Name : null))
            .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => src.Zone != null ? src.Zone.Name : null));

        CreateMap<CreateReportDTO, Report>()
            .ForMember(dest => dest.GeneratedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateReportDTO, Report>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
