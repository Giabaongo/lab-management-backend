using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Extensions;
using LabManagement.Common.Models;
using LabManagement.Common.Exceptions;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.BLL.Implementations
{
    public class LabEventService : ILabEventService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LabEventService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<LabEventCreationResultDTO> CreateLabEventAsync(CreateLabEventDTO createLabEventDTO)
        {
            var labEvent = _mapper.Map<LabEvent>(createLabEventDTO);
            var cancelledBookings = new List<CancelledItemDTO>();
            var cancelledEvents = new List<CancelledItemDTO>();

            if (labEvent.IsHighPriority)
            {
                // If high priority event, auto-cancel conflicting bookings and low-priority events
                var (bookings, events) = await CancelConflictingReservationsAsync(
                    labEvent.LabId,
                    labEvent.ZoneId,
                    labEvent.StartTime,
                    labEvent.EndTime,
                    null); // null = new event, no eventId to exclude

                cancelledBookings = bookings;
                cancelledEvents = events;
            }
            else
            {
                // If normal priority event, check for conflicts with high priority events
                var hasHighPriorityConflict = await HasHighPriorityEventConflictAsync(
                    labEvent.LabId,
                    labEvent.ZoneId,
                    labEvent.StartTime,
                    labEvent.EndTime,
                    null);

                if (hasHighPriorityConflict)
                {
                    throw new BadRequestException("Cannot create event: conflicts with a high priority event");
                }
            }

            await _unitOfWork.LabEvents.AddAsync(labEvent);
            await _unitOfWork.SaveChangesAsync();

            return new LabEventCreationResultDTO
            {
                Event = _mapper.Map<LabEventDTO>(labEvent),
                CancelledBookings = cancelledBookings,
                CancelledEvents = cancelledEvents
            };
        }

        public async Task<bool> DeleteLabEventAsync(int id)
        {
            var labEvent = await _unitOfWork.LabEvents.GetByIdAsync(id);
            if (labEvent == null) return false;

            await _unitOfWork.LabEvents.DeleteAsync(labEvent);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LabEventDTO>> GetAllLabEventsAsync()
        {
            var labEvents = await _unitOfWork.LabEvents.GetAllAsync();
            return _mapper.Map<IEnumerable<LabEventDTO>>(labEvents);
        }

        public async Task<PagedResult<LabEventDTO>> GetLabEventsAsync(QueryParameters queryParams)
        {
            var query = _unitOfWork.LabEvents.GetLabEventsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.Title.ToLower().Contains(term) ||
                    (e.Description != null && e.Description.ToLower().Contains(term)) ||
                    e.EventId.ToString().Contains(term) ||
                    e.LabId.ToString().Contains(term) ||
                    e.ZoneId.ToString().Contains(term) ||
                    e.OrganizerId.ToString().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "title" => queryParams.IsDescending ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
                    "starttime" => queryParams.IsDescending ? query.OrderByDescending(e => e.StartTime) : query.OrderBy(e => e.StartTime),
                    "endtime" => queryParams.IsDescending ? query.OrderByDescending(e => e.EndTime) : query.OrderBy(e => e.EndTime),
                    "status" => queryParams.IsDescending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
                    "createdat" => queryParams.IsDescending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt),
                    "labid" => queryParams.IsDescending ? query.OrderByDescending(e => e.LabId) : query.OrderBy(e => e.LabId),
                    "zoneid" => queryParams.IsDescending ? query.OrderByDescending(e => e.ZoneId) : query.OrderBy(e => e.ZoneId),
                    _ => query.OrderBy(e => e.EventId)
                };
            }
            else
            {
                query = query.OrderBy(e => e.EventId);
            }

            var pagedLabEvents = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

            return new PagedResult<LabEventDTO>
            {
                Items = _mapper.Map<IEnumerable<LabEventDTO>>(pagedLabEvents.Items),
                PageNumber = pagedLabEvents.PageNumber,
                PageSize = pagedLabEvents.PageSize,
                TotalCount = pagedLabEvents.TotalCount
            };
        }

        public async Task<LabEventDTO?> GetLabEventByIdAsync(int id)
        {
            var labEvent = await _unitOfWork.LabEvents.GetByIdAsync(id);
            return labEvent == null ? null : _mapper.Map<LabEventDTO>(labEvent);
        }

        public async Task<bool> LabEventExistsAsync(int id)
        {
            return await _unitOfWork.LabEvents.ExistsAsync(e => e.EventId == id);
        }

        public async Task<LabEventCreationResultDTO?> UpdateLabEventAsync(int id, UpdateLabEventDTO updateLabEventDTO)
        {
            var labEvent = await _unitOfWork.LabEvents.GetByIdAsync(id);
            if (labEvent == null) return null;

            var startTime = updateLabEventDTO.StartTime ?? labEvent.StartTime;
            var endTime = updateLabEventDTO.EndTime ?? labEvent.EndTime;
            var labId = updateLabEventDTO.LabId ?? labEvent.LabId;
            var zoneId = updateLabEventDTO.ZoneId ?? labEvent.ZoneId;
            var willBeHighPriority = updateLabEventDTO.IsHighPriority ?? labEvent.IsHighPriority;

            var cancelledBookings = new List<CancelledItemDTO>();
            var cancelledEvents = new List<CancelledItemDTO>();

            if (willBeHighPriority)
            {
                // If changing to (or staying as) high priority, cancel conflicting bookings and events
                var (bookings, events) = await CancelConflictingReservationsAsync(labId, zoneId, startTime, endTime, id);
                cancelledBookings = bookings;
                cancelledEvents = events;
            }
            else
            {
                // If normal priority, check for conflicts with high priority events
                var hasHighPriorityConflict = await HasHighPriorityEventConflictAsync(
                    labId, zoneId, startTime, endTime, id);

                if (hasHighPriorityConflict)
                {
                    throw new BadRequestException("Cannot update event: conflicts with a high priority event");
                }
            }

            _mapper.Map(updateLabEventDTO, labEvent);
            await _unitOfWork.LabEvents.UpdateAsync(labEvent);
            await _unitOfWork.SaveChangesAsync();

            return new LabEventCreationResultDTO
            {
                Event = _mapper.Map<LabEventDTO>(labEvent),
                CancelledBookings = cancelledBookings,
                CancelledEvents = cancelledEvents
            };
        }

        private async Task<(List<CancelledItemDTO> bookings, List<CancelledItemDTO> events)> CancelConflictingReservationsAsync(
            int labId, int zoneId, DateTime startTime, DateTime endTime, int? currentEventId)
        {
            var cancelledBookingDTOs = new List<CancelledItemDTO>();
            var cancelledEventDTOs = new List<CancelledItemDTO>();

            // 1. Cancel all conflicting bookings
            var conflictingBookings = await _unitOfWork.Bookings
                .GetBookingsQueryable()
                .Where(b => b.LabId == labId &&
                           b.ZoneId == zoneId &&
                           b.StartTime < endTime &&
                           b.EndTime > startTime &&
                           b.Status == 1) // 1 = Confirmed
                .ToListAsync();

            foreach (var booking in conflictingBookings)
            {
                booking.Status = 2; // 2 = Cancelled
                await _unitOfWork.Bookings.UpdateAsync(booking);

                cancelledBookingDTOs.Add(new CancelledItemDTO
                {
                    Id = booking.BookingId,
                    Title = $"Booking #{booking.BookingId}",
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    Notes = booking.Notes
                });
            }

            // 2. Cancel all conflicting low-priority events
            var conflictingEvents = await _unitOfWork.LabEvents
                .GetLabEventsQueryable()
                .Where(e => e.LabId == labId &&
                           e.ZoneId == zoneId &&
                           !e.IsHighPriority && // Only cancel low-priority events
                           e.StartTime < endTime &&
                           e.EndTime > startTime &&
                           e.Status == 1 && // 1 = Confirmed/Active
                           (currentEventId == null || e.EventId != currentEventId)) // Exclude current event when updating
                .ToListAsync();

            foreach (var labEvent in conflictingEvents)
            {
                labEvent.Status = 2; // 2 = Cancelled
                await _unitOfWork.LabEvents.UpdateAsync(labEvent);

                cancelledEventDTOs.Add(new CancelledItemDTO
                {
                    Id = labEvent.EventId,
                    Title = labEvent.Title,
                    StartTime = labEvent.StartTime,
                    EndTime = labEvent.EndTime,
                    Notes = labEvent.Description
                });
            }

            // Save all changes if any conflicts found
            if (conflictingBookings.Any() || conflictingEvents.Any())
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return (cancelledBookingDTOs, cancelledEventDTOs);
        }

        private async Task<bool> HasHighPriorityEventConflictAsync(int labId, int zoneId, DateTime startTime, DateTime endTime, int? currentEventId)
        {
            return await _unitOfWork.LabEvents
                .GetLabEventsQueryable()
                .AnyAsync(e => e.LabId == labId &&
                              e.ZoneId == zoneId &&
                              e.IsHighPriority &&
                              e.StartTime < endTime &&
                              e.EndTime > startTime &&
                              e.Status == 1 && // 1 = Active/Confirmed
                              (currentEventId == null || e.EventId != currentEventId));
        }
    }
}
