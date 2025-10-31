using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Implementations
{
    public class LabZoneService : ILabZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LabZoneService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<LabZoneDTO> CreateLabZoneAsync(CreateLabZoneDTO createLabZoneDTO)
        {   
            var labZone = _mapper.Map<LabZone>(createLabZoneDTO);
            await _unitOfWork.LabZones.AddAsync(labZone);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<LabZoneDTO>(labZone);
        }

        public async Task<bool> DeleteLabZoneAsync(int id)
        {
            var labZone = await _unitOfWork.LabZones.GetByIdAsync(id);
            if(labZone == null) return false; 

            await _unitOfWork.LabZones.DeleteAsync(labZone);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LabZoneDTO>> GetAllLabZonesAsync()
        {
            var labZones = await _unitOfWork.LabZones.GetAllAsync();
            return _mapper.Map<IEnumerable<LabZoneDTO>>(labZones);
        }

        public async Task<LabZoneDTO?> GetLabZoneByIdAsync(int id)
        {
            var labZone = await _unitOfWork.LabZones.GetByIdAsync(id);
            return labZone == null ? null : _mapper.Map<LabZoneDTO?>(labZone);
        }

        public async Task<bool> LabZoneNameExistsAsync(string name)
        {
            return await _unitOfWork.LabZones.ExistsAsync(lz => lz.Name == name);
        }

        public async Task<bool> LabZoneIdExistsAsync(int id)
        {
            return await _unitOfWork.LabZones.ExistsAsync(lz => lz.LabId == id);
        }
        public async Task<LabZoneDTO?> UpdateLabZoneAsync(int id, UpdateLabZoneDTO updateLabZoneDTO)
        {
            var labZone = await _unitOfWork.LabZones.GetByIdAsync(id);
            if (labZone != null){
                _mapper.Map(updateLabZoneDTO, labZone);
                await _unitOfWork.LabZones.UpdateAsync(labZone);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<LabZoneDTO>(labZone);    
            }
            return null;
            ;
            
        }
    }
}
