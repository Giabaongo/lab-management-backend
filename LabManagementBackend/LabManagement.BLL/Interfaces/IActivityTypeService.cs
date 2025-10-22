using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Interfaces
{
    public interface IActivityTypeService
    {
        Task<IEnumerable<ActivityTypeDTO>> GetAllActivityTypesAsync();
        Task<ActivityTypeDTO?> GetActivityTypeByIdAsync(int id);
        Task<ActivityTypeDTO> CreateActivityTypeAsync(CreateActivityTypeDTO createActivityTypeDTO);
        Task<ActivityTypeDTO?> UpdateActivityTypeAsync(int id, UpdateActivityTypeDTO updateActivityTypeDTO);
        Task<bool> DeleteActivityTypeAsync(int id);
        Task<bool> ActivityTypeExistsAsync(int id);
    }
}
