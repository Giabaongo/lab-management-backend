using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Extensions;
using LabManagement.Common.Models;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Collections.Generic;
using System.Linq;

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

        public async Task<LabEventDTO> CreateLabEventAsync(CreateLabEventDTO createLabEventDTO)
        {
            var labEvent = _mapper.Map<LabEvent>(createLabEventDTO);
            await _unitOfWork.LabEvents.AddAsync(labEvent);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LabEventDTO>(labEvent);
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

        public async Task<LabEventDTO?> UpdateLabEventAsync(int id, UpdateLabEventDTO updateLabEventDTO)
        {
            var labEvent = await _unitOfWork.LabEvents.GetByIdAsync(id);
            if (labEvent == null) return null;

            _mapper.Map(updateLabEventDTO, labEvent);
            await _unitOfWork.LabEvents.UpdateAsync(labEvent);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LabEventDTO>(labEvent);
        }
    }
}
