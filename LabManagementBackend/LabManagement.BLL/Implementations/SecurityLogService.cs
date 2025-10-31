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
    public class SecurityLogService : ISecurityLogService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SecurityLogService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SecurityLogDTO> CreateSecurityLogAsync(CreateSecurityLogDTO createSecurityLogDTO)
        {
            var securityLog = _mapper.Map<SecurityLog>(createSecurityLogDTO);
            await _unitOfWork.SecurityLogs.AddAsync(securityLog);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SecurityLogDTO>(securityLog);
        }

        public async Task<bool> DeleteSecurityLogAsync(int id)
        {
            var securityLog = await _unitOfWork.SecurityLogs.GetByIdAsync(id);
            if (securityLog == null) return false;

            await _unitOfWork.SecurityLogs.DeleteAsync(securityLog);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SecurityLogDTO>> GetAllSecurityLogsAsync()
        {
            var securityLogs = await _unitOfWork.SecurityLogs.GetAllAsync();
            return _mapper.Map<IEnumerable<SecurityLogDTO>>(securityLogs);
        }

        public async Task<PagedResult<SecurityLogDTO>> GetSecurityLogsAsync(QueryParameters queryParams)
        {
            var query = _unitOfWork.SecurityLogs.GetSecurityLogsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(s =>
                    (s.Notes != null && s.Notes.ToLower().Contains(term)) ||
                    (s.PhotoUrl != null && s.PhotoUrl.ToLower().Contains(term)) ||
                    s.LogId.ToString().Contains(term) ||
                    s.SecurityId.ToString().Contains(term) ||
                    s.EventId.ToString().Contains(term) ||
                    s.ActionType.ToString().Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "loggedat" => queryParams.IsDescending ? query.OrderByDescending(s => s.LoggedAt) : query.OrderBy(s => s.LoggedAt),
                    "actiontype" => queryParams.IsDescending ? query.OrderByDescending(s => s.ActionType) : query.OrderBy(s => s.ActionType),
                    "securityid" => queryParams.IsDescending ? query.OrderByDescending(s => s.SecurityId) : query.OrderBy(s => s.SecurityId),
                    "eventid" => queryParams.IsDescending ? query.OrderByDescending(s => s.EventId) : query.OrderBy(s => s.EventId),
                    _ => query.OrderBy(s => s.LogId)
                };
            }
            else
            {
                query = query.OrderBy(s => s.LogId);
            }

            var pagedSecurityLogs = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

            return new PagedResult<SecurityLogDTO>
            {
                Items = _mapper.Map<IEnumerable<SecurityLogDTO>>(pagedSecurityLogs.Items),
                PageNumber = pagedSecurityLogs.PageNumber,
                PageSize = pagedSecurityLogs.PageSize,
                TotalCount = pagedSecurityLogs.TotalCount
            };
        }

        public async Task<SecurityLogDTO?> GetSecurityLogByIdAsync(int id)
        {
            var securityLog = await _unitOfWork.SecurityLogs.GetByIdAsync(id);
            return securityLog == null ? null : _mapper.Map<SecurityLogDTO>(securityLog);
        }

        public async Task<bool> SecurityLogExistsAsync(int id)
        {
            return await _unitOfWork.SecurityLogs.ExistsAsync(s => s.LogId == id);
        }

        public async Task<SecurityLogDTO?> UpdateSecurityLogAsync(int id, UpdateSecurityLogDTO updateSecurityLogDTO)
        {
            var securityLog = await _unitOfWork.SecurityLogs.GetByIdAsync(id);
            if (securityLog == null) return null;

            _mapper.Map(updateSecurityLogDTO, securityLog);
            await _unitOfWork.SecurityLogs.UpdateAsync(securityLog);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SecurityLogDTO>(securityLog);
        }
    }
}
