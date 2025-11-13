using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Extensions;
using LabManagement.Common.Exceptions;
using LabManagement.Common.Models;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.BLL.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BookingDTO> CreateBookingAsync(CreateBookingDTO createBookingDTO, int requesterId, Constant.UserRole requesterRole)
        {
            await EnsureLabAccessAsync(createBookingDTO.LabId, requesterId, requesterRole);

            if (requesterRole == Constant.UserRole.Member && createBookingDTO.UserId != requesterId)
            {
                throw new UnauthorizedException("Members can only create bookings for themselves");
            }

            var booking = _mapper.Map<Booking>(createBookingDTO);
            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return false;

            await _unitOfWork.Bookings.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync()
        {
            var bookings = await _unitOfWork.Bookings.GetAllAsync();
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<PagedResult<BookingDTO>> GetBookingsAsync(QueryParameters queryParams)
        {
            var query = _unitOfWork.Bookings.GetBookingsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(b =>
                    (b.Notes != null && b.Notes.ToLower().Contains(term)) ||
                    b.BookingId.ToString().Contains(term) ||
                    b.UserId.ToString().Contains(term) ||
                    b.LabId.ToString().Contains(term) ||
                    b.ZoneId.ToString().Contains(term) ||
                    b.Status.ToString().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "starttime" => queryParams.IsDescending ? query.OrderByDescending(b => b.StartTime) : query.OrderBy(b => b.StartTime),
                    "endtime" => queryParams.IsDescending ? query.OrderByDescending(b => b.EndTime) : query.OrderBy(b => b.EndTime),
                    "status" => queryParams.IsDescending ? query.OrderByDescending(b => b.Status) : query.OrderBy(b => b.Status),
                    "createdat" => queryParams.IsDescending ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt),
                    "userid" => queryParams.IsDescending ? query.OrderByDescending(b => b.UserId) : query.OrderBy(b => b.UserId),
                    "labid" => queryParams.IsDescending ? query.OrderByDescending(b => b.LabId) : query.OrderBy(b => b.LabId),
                    "zoneid" => queryParams.IsDescending ? query.OrderByDescending(b => b.ZoneId) : query.OrderBy(b => b.ZoneId),
                    _ => query.OrderBy(b => b.BookingId)
                };
            }
            else
            {
                query = query.OrderBy(b => b.BookingId);
            }

            var pagedBookings = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

            return new PagedResult<BookingDTO>
            {
                Items = _mapper.Map<IEnumerable<BookingDTO>>(pagedBookings.Items),
                PageNumber = pagedBookings.PageNumber,
                PageSize = pagedBookings.PageSize,
                TotalCount = pagedBookings.TotalCount
            };
        }

        public async Task<BookingDTO?> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            return booking == null ? null : _mapper.Map<BookingDTO>(booking);
        }

        public async Task<bool> BookingExistsAsync(int id)
        {
            return await _unitOfWork.Bookings.ExistsAsync(b => b.BookingId == id);
        }

        public async Task<BookingDTO?> UpdateBookingAsync(int id, UpdateBookingDTO updateBookingDTO)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return null;

            _mapper.Map(updateBookingDTO, booking);
            await _unitOfWork.Bookings.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<IEnumerable<AvailableSlotDTO>> GetAvailableSlotsAsync(AvailableSlotQueryDTO query, int requesterId, Constant.UserRole requesterRole)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (query.SlotDurationMinutes <= 0)
                throw new BadRequestException("Slot duration must be greater than zero.");

            if (query.DayStart < TimeSpan.Zero || query.DayStart >= TimeSpan.FromHours(24))
                throw new BadRequestException("DayStart must be between 00:00 and 24:00.");

            if (query.DayEnd <= TimeSpan.Zero || query.DayEnd > TimeSpan.FromHours(24))
                throw new BadRequestException("DayEnd must be greater than 00:00 and not exceed 24:00.");

            if (query.DayEnd <= query.DayStart)
                throw new BadRequestException("DayEnd must be after DayStart.");

            await EnsureLabAccessAsync(query.LabId, requesterId, requesterRole);

            var slotDuration = TimeSpan.FromMinutes(query.SlotDurationMinutes);
            if (slotDuration >= query.DayEnd - query.DayStart)
                throw new BadRequestException("Slot duration must be shorter than the working window.");

            var dayStart = query.Date.Date.Add(query.DayStart);
            var dayEnd = query.Date.Date.Add(query.DayEnd);

            var bookings = await _unitOfWork.Bookings.GetBookingsInRangeAsync(query.LabId, query.ZoneId, dayStart, dayEnd);

            var availableSlots = new List<AvailableSlotDTO>();

            for (var slotStart = dayStart; slotStart.Add(slotDuration) <= dayEnd; slotStart = slotStart.Add(slotDuration))
            {
                var slotEnd = slotStart.Add(slotDuration);
                var overlaps = bookings.Any(b => slotStart < b.EndTime && slotEnd > b.StartTime);
                if (!overlaps)
                {
                    availableSlots.Add(new AvailableSlotDTO
                    {
                        StartTime = slotStart,
                        EndTime = slotEnd
                    });
                }
            }

            return availableSlots;
        }

        private static bool IsElevatedRole(Constant.UserRole role)
        {
            return role == Constant.UserRole.Admin ||
                   role == Constant.UserRole.SchoolManager;
        }

        private async Task EnsureLabAccessAsync(int labId, int requesterId, Constant.UserRole requesterRole)
        {
            var lab = await _unitOfWork.Labs
                .GetLabsQueryable()
                .Include(l => l.Department)
                    .ThenInclude(d => d.UserDepartments)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LabId == labId);

            if (lab == null)
            {
                throw new NotFoundException("Lab", labId);
            }

            if (IsElevatedRole(requesterRole))
            {
                return;
            }

            var hasAccess = lab.Department.IsPublic ||
                            lab.ManagerId == requesterId ||
                            lab.Department.UserDepartments.Any(ud => ud.UserId == requesterId);

            if (!hasAccess)
            {
                throw new UnauthorizedException("You do not have permission to access this lab");
            }
        }
    }
}
