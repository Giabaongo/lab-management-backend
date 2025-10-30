using LabManagement.BLL.DTOs;
using LabManagement.Common.Models;

namespace LabManagement.BLL.Interfaces
{
    public interface ILabService
    {
        Task<IEnumerable<LabDTO>> GetAllLabsAsync();
        Task<PagedResult<LabDTO>> GetLabsAsync(QueryParameters queryParams);
        Task<LabDTO?> GetLabByIdAsync(int id);
        Task<LabDTO> CreateLabAsync(CreateLabDTO createLabDTO);
        Task<LabDTO?> UpdateLabAsync(UpdateLabDTO updateLabDTO, string name);
        Task<bool> DeleteLabAsync(int id);
        Task<bool> LabExistsAsync(string name);
    }
}
