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
