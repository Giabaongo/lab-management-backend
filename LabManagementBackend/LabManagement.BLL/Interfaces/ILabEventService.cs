using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Interfaces
{
    public interface ILabEventService
    {
        Task<IEnumerable<LabEventDTO>> GetAllLabEventsAsync();
        Task<LabEventDTO?> GetLabEventByIdAsync(int id);
        Task<LabEventDTO> CreateLabEventAsync(CreateLabEventDTO createLabEventDTO);
        Task<LabEventDTO?> UpdateLabEventAsync(int id, UpdateLabEventDTO updateLabEventDTO);
        Task<bool> DeleteLabEventAsync(int id);
        Task<bool> LabEventExistsAsync(int id);
    }
}
