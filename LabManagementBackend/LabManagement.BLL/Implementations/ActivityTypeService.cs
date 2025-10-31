using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Implementations
{
    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ActivityTypeService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivityTypeDTO> CreateActivityTypeAsync(CreateActivityTypeDTO createActivityTypeDTO)
        {
            var activityType = _mapper.Map<ActivityType>(createActivityTypeDTO);
            await _unitOfWork.ActivityTypes.AddAsync(activityType);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ActivityTypeDTO>(activityType);
        }

        public async Task<bool> DeleteActivityTypeAsync(int id)
        {
            var activityType = await _unitOfWork.ActivityTypes.GetByIdAsync(id);
            if (activityType == null) return false;

            await _unitOfWork.ActivityTypes.DeleteAsync(activityType);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ActivityTypeDTO>> GetAllActivityTypesAsync()
        {
            var activityTypes = await _unitOfWork.ActivityTypes.GetAllAsync();
            return _mapper.Map<IEnumerable<ActivityTypeDTO>>(activityTypes);
        }

        public async Task<ActivityTypeDTO?> GetActivityTypeByIdAsync(int id)
        {
            var activityType = await _unitOfWork.ActivityTypes.GetByIdAsync(id);
            return activityType == null ? null : _mapper.Map<ActivityTypeDTO>(activityType);
        }

        public async Task<bool> ActivityTypeExistsAsync(int id)
        {
            return await _unitOfWork.ActivityTypes.ExistsAsync(a => a.ActivityTypeId == id);
        }

        public async Task<ActivityTypeDTO?> UpdateActivityTypeAsync(int id, UpdateActivityTypeDTO updateActivityTypeDTO)
        {
            var activityType = await _unitOfWork.ActivityTypes.GetByIdAsync(id);
            if (activityType == null) return null;

            _mapper.Map(updateActivityTypeDTO, activityType);
            await _unitOfWork.ActivityTypes.UpdateAsync(activityType);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ActivityTypeDTO>(activityType);
        }
    }
}
