using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDTO>();
            CreateMap<CreateBookingDTO, Booking>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateBookingDTO, Booking>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
