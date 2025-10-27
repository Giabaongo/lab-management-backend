using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

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
