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
            var lab = await EnsureLabAccessAsync(createBookingDTO.LabId, requesterId, requesterRole);
            await EnsureDepartmentQuotaForBookingAsync(lab.DepartmentId, requesterId, requesterRole);
            await EnsureZoneBelongsToLabAsync(createBookingDTO.ZoneId, lab.LabId);

            if (requesterRole == Constant.UserRole.Member && createBookingDTO.UserId != requesterId)
            {
                throw new UnauthorizedException("Members can only create bookings for themselves");
            }

            var booking = _mapper.Map<Booking>(createBookingDTO);
            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<bool> DeleteBookingAsync(int id, int requesterId, Constant.UserRole requesterRole)
        {
            if (!IsElevatedRole(requesterRole))
            {
                throw new UnauthorizedException("Only administrators can delete bookings");
            }

            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null) return false;

            await _unitOfWork.Bookings.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDTO>> GetAllBookingsAsync(int requesterId, Constant.UserRole requesterRole)
        {
            var query = _unitOfWork.Bookings
                .GetBookingsQueryable()
                .Include(b => b.Lab)
                    .ThenInclude(l => l.Department)
                .Include(b => b.Zone)
                .AsNoTracking();

            query = ApplyBookingVisibilityFilter(query, requesterId, requesterRole);

            var bookings = await query.ToListAsync();
            return _mapper.Map<IEnumerable<BookingDTO>>(bookings);
        }

        public async Task<PagedResult<BookingDTO>> GetBookingsAsync(QueryParameters queryParams, int requesterId, Constant.UserRole requesterRole)
        {
            var query = _unitOfWork.Bookings
                .GetBookingsQueryable()
                .Include(b => b.Lab)
                    .ThenInclude(l => l.Department)
                .Include(b => b.Zone)
                .AsNoTracking();

            query = ApplyBookingVisibilityFilter(query, requesterId, requesterRole);

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(b =>
                    (b.Notes != null && b.Notes.ToLower().Contains(term)) ||
                    b.BookingId.ToString().Contains(term) ||
                    b.UserId.ToString().Contains(term) ||
                    b.Lab.Name.ToLower().Contains(term) ||
                    b.Zone.Name.ToLower().Contains(term));
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
                    "lab" => queryParams.IsDescending ? query.OrderByDescending(b => b.Lab.Name) : query.OrderBy(b => b.Lab.Name),
                    "zone" => queryParams.IsDescending ? query.OrderByDescending(b => b.Zone.Name) : query.OrderBy(b => b.Zone.Name),
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

        public async Task<BookingDTO?> GetBookingByIdAsync(int id, int requesterId, Constant.UserRole requesterRole)
        {
            var booking = await _unitOfWork.Bookings
                .GetBookingsQueryable()
                .Include(b => b.Lab)
                    .ThenInclude(l => l.Department)
                .Include(b => b.Zone)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return null;
            }

            EnsureBookingAccess(booking, requesterId, requesterRole);
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO?> UpdateBookingAsync(int id, UpdateBookingDTO updateBookingDTO, int requesterId, Constant.UserRole requesterRole)
        {
            var booking = await _unitOfWork.Bookings
                .GetBookingsQueryable()
                .Include(b => b.Lab)
                    .ThenInclude(l => l.Department)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null) return null;

            EnsureBookingAccess(booking, requesterId, requesterRole);

            if (updateBookingDTO.UserId.HasValue && updateBookingDTO.UserId.Value != booking.UserId && !IsElevatedRole(requesterRole))
            {
                throw new UnauthorizedException("Only administrators can reassign bookings to another user");
            }

            var targetLabId = updateBookingDTO.LabId ?? booking.LabId;
            Lab? updatedLab = null;

            if (updateBookingDTO.LabId.HasValue && updateBookingDTO.LabId.Value != booking.LabId)
            {
                updatedLab = await EnsureLabAccessAsync(updateBookingDTO.LabId.Value, requesterId, requesterRole);
                await EnsureDepartmentQuotaForBookingAsync(updatedLab.DepartmentId, requesterId, requesterRole);
            }

            if (updateBookingDTO.ZoneId.HasValue)
            {
                await EnsureZoneBelongsToLabAsync(updateBookingDTO.ZoneId.Value, targetLabId);
            }

            _mapper.Map(updateBookingDTO, booking);

            if (updatedLab != null)
            {
                booking.Lab = updatedLab;
            }

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

            var lab = await EnsureLabAccessAsync(query.LabId, requesterId, requesterRole);
            await EnsureDepartmentQuotaForBookingAsync(lab.DepartmentId, requesterId, requesterRole);
            await EnsureZoneBelongsToLabAsync(query.ZoneId, lab.LabId);

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

        private static IQueryable<Booking> ApplyBookingVisibilityFilter(IQueryable<Booking> query, int requesterId, Constant.UserRole requesterRole)
        {
            if (IsElevatedRole(requesterRole))
            {
                return query;
            }

            if (requesterRole == Constant.UserRole.LabManager)
            {
                return query.Where(b => b.UserId == requesterId || b.Lab.ManagerId == requesterId);
            }

            return query.Where(b => b.UserId == requesterId);
        }

        private void EnsureBookingAccess(Booking booking, int requesterId, Constant.UserRole requesterRole)
        {
            if (IsElevatedRole(requesterRole))
            {
                return;
            }

            if (requesterRole == Constant.UserRole.LabManager && booking.Lab.ManagerId == requesterId)
            {
                return;
            }

            if (booking.UserId != requesterId)
            {
                throw new UnauthorizedException("You do not have permission to view or modify this booking");
            }
        }

        private async Task EnsureZoneBelongsToLabAsync(int zoneId, int labId)
        {
            var zone = await _unitOfWork.LabZones.GetByIdAsync(zoneId);
            if (zone == null)
            {
                throw new NotFoundException("Lab zone", zoneId);
            }

            if (zone.LabId != labId)
            {
                throw new BadRequestException("Selected zone does not belong to the specified lab");
            }
        }

        private async Task EnsureDepartmentQuotaForBookingAsync(int departmentId, int requesterId, Constant.UserRole requesterRole)
        {
            if (requesterRole != Constant.UserRole.Member)
            {
                return;
            }

            var membershipDepartments = await _unitOfWork.UserDepartments
                .GetUserDepartmentsQueryable()
                .Where(ud => ud.UserId == requesterId)
                .Select(ud => ud.DepartmentId)
                .ToListAsync();

            var futureBookingDepartments = await _unitOfWork.Bookings
                .GetBookingsQueryable()
                .Include(b => b.Lab)
                .AsNoTracking()
                .Where(b => b.UserId == requesterId && b.EndTime >= DateTime.UtcNow)
                .Select(b => b.Lab.DepartmentId)
                .ToListAsync();

            var departmentSet = new HashSet<int>(membershipDepartments);
            foreach (var deptId in futureBookingDepartments)
            {
                departmentSet.Add(deptId);
            }

            if (!departmentSet.Contains(departmentId))
            {
                departmentSet.Add(departmentId);
            }

            if (departmentSet.Count > Constant.MaxDepartmentsPerMember)
            {
                throw new BadRequestException($"Members can only engage with up to {Constant.MaxDepartmentsPerMember} departments at a time. Please cancel an existing booking or unregister from a department before booking another lab.");
            }
        }

        private async Task<Lab> EnsureLabAccessAsync(int labId, int requesterId, Constant.UserRole requesterRole)
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
                return lab;
            }

            var hasAccess = lab.Department.IsPublic ||
                            lab.ManagerId == requesterId ||
                            lab.Department.UserDepartments.Any(ud => ud.UserId == requesterId);

            if (!hasAccess)
            {
                throw new UnauthorizedException("You do not have permission to access this lab");
            }

            return lab;
        }
    }
}
