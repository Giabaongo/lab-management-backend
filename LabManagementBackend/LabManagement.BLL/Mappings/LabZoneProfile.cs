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
    public class LabZoneProfile : Profile
    {
        public LabZoneProfile()
        {
            //CreateMap<LabZone, LabZoneDTO>();
            //          source destination
            CreateMap<LabZone, LabZoneDTO>()
                .ForMember(dest => dest.ZoneId, opt => opt.MapFrom(src => src.ZoneId))
                .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                //.ForMember(dest => dest.VoucherCode, opt => opt.MapFrom(src => src.Voucher.Code))
                ;

            CreateMap<CreateLabZoneDTO, LabZone>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                ;

            CreateMap<UpdateLabZoneDTO, LabZone>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
