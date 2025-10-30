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
    public class LabService : ILabService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LabService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<LabDTO> CreateLabAsync(CreateLabDTO createLabDTO)
        {
            var lab = _mapper.Map<Lab>(createLabDTO);
            await _unitOfWork.Labs.AddAsync(lab);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LabDTO>(lab);
        }

        public async Task<bool> DeleteLabAsync(int id)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(id);
            if(lab == null) return false;

            await _unitOfWork.Labs.DeleteAsync(lab);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LabDTO>> GetAllLabsAsync()
        {
            var labs = await _unitOfWork.Labs.GetAllAsync();
            return _mapper.Map<IEnumerable<LabDTO>>(labs);
        }

        public async Task<PagedResult<LabDTO>> GetLabsAsync(QueryParameters queryParams)
        {
            var query = _unitOfWork.Labs.GetLabsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(l =>
                    l.Name.ToLower().Contains(term) ||
                    (l.Location != null && l.Location.ToLower().Contains(term)) ||
                    (l.Description != null && l.Description.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "name" => queryParams.IsDescending ? query.OrderByDescending(l => l.Name) : query.OrderBy(l => l.Name),
                    "location" => queryParams.IsDescending ? query.OrderByDescending(l => l.Location) : query.OrderBy(l => l.Location),
                    "managerid" => queryParams.IsDescending ? query.OrderByDescending(l => l.ManagerId) : query.OrderBy(l => l.ManagerId),
                    _ => query.OrderBy(l => l.LabId)
                };
            }
            else
            {
                query = query.OrderBy(l => l.LabId);
            }

            var pagedLabs = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

            return new PagedResult<LabDTO>
            {
                Items = _mapper.Map<IEnumerable<LabDTO>>(pagedLabs.Items),
                PageNumber = pagedLabs.PageNumber,
                PageSize = pagedLabs.PageSize,
                TotalCount = pagedLabs.TotalCount
            };
        }

        public async Task<LabDTO?> GetLabByIdAsync(int id)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(id);
            return lab == null ? null : _mapper.Map<LabDTO>(lab);
        }

        public async Task<bool> LabExistsAsync(string name)
        {
            return await _unitOfWork.Labs.ExistsAsync(l => l.Name == name);
        }

        public async Task<LabDTO?> UpdateLabAsync(UpdateLabDTO updateLabDTO, string name)
        {
            var lab = await _unitOfWork.Labs.FirstOrDefaultAsync(l => l.Name == name);
            if (lab != null)
            {
                _mapper.Map(updateLabDTO, lab);
                await _unitOfWork.Labs.UpdateAsync(lab);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<LabDTO>(lab);
            }
            return null;
        }
    }
}
