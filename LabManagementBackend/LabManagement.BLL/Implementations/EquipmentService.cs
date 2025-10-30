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
    public class EquipmentService : IEquipmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EquipmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<EquipmentDTO> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO)
        {
            var equipment = _mapper.Map<Equipment>(createEquipmentDTO);
            await _unitOfWork.Equipment.AddAsync(equipment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EquipmentDTO>(equipment);
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var equipment = await _unitOfWork.Equipment.GetByIdAsync(id);
            if (equipment == null) return false;

            await _unitOfWork.Equipment.DeleteAsync(equipment);
            await _unitOfWork.SaveChangesAsync();
            return true;    

        }

        public async Task<bool> EquipmentCodeExistsAsync(string code)
        {
            //var equipmentExist = await _unitOfWork.Equipment.FirstOrDefaultAsync(x => x.Code == code); => check if restaurant open by order pizza not optimal
            return await _unitOfWork.Equipment.ExistsAsync(x => x.Code == code); //=> check if the restaurant open by asking .
            
        }

        public async Task<bool> EquipmentStatusExistsAsync(int status)
        {
            return await _unitOfWork.Equipment.ExistsAsync(x => x.Status == status);
        }

        public async Task<IEnumerable<EquipmentDTO>> GetAllEquipmentAsync()
        {
            var equipments =  await _unitOfWork.Equipment.GetAllAsync();
            return _mapper.Map<IEnumerable<EquipmentDTO>>(equipments);
        }

        public async Task<PagedResult<EquipmentDTO>> GetEquipmentAsync(QueryParameters queryParams)
        {
            var query = _unitOfWork.Equipment.GetEquipmentQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(term) ||
                    e.Code.ToLower().Contains(term) ||
                    (e.Description != null && e.Description.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "name" => queryParams.IsDescending ? query.OrderByDescending(e => e.Name) : query.OrderBy(e => e.Name),
                    "code" => queryParams.IsDescending ? query.OrderByDescending(e => e.Code) : query.OrderBy(e => e.Code),
                    "status" => queryParams.IsDescending ? query.OrderByDescending(e => e.Status) : query.OrderBy(e => e.Status),
                    "labid" => queryParams.IsDescending ? query.OrderByDescending(e => e.LabId) : query.OrderBy(e => e.LabId),
                    _ => query.OrderBy(e => e.EquipmentId)
                };
            }
            else
            {
                query = query.OrderBy(e => e.EquipmentId);
            }

            var pagedEquipment = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

            return new PagedResult<EquipmentDTO>
            {
                Items = _mapper.Map<IEnumerable<EquipmentDTO>>(pagedEquipment.Items),
                PageNumber = pagedEquipment.PageNumber,
                PageSize = pagedEquipment.PageSize,
                TotalCount = pagedEquipment.TotalCount
            };
        }

        public async Task<EquipmentDTO?> GetEquipmentByIdAsync(int id)
        {
            var equipment = await _unitOfWork.Equipment.GetByIdAsync(id);
            return equipment == null ? null : _mapper.Map<EquipmentDTO?>(equipment);
        }

        public async Task<EquipmentDTO?> UpdateEquipmentAsync(int id, UpdateEquipmentDTO updateEquipmentDTO)
        {
            var equipment =  await _unitOfWork.Equipment.GetByIdAsync(id);
            if (equipment == null) return null;
            _mapper.Map(updateEquipmentDTO, equipment);
            await _unitOfWork.Equipment.UpdateAsync(equipment);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<EquipmentDTO>(equipment);
        }
    }
}
