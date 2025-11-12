using LabManagement.BLL.DTOs;
using LabManagement.Common.Constants;
using LabManagement.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface ILabService
    {
        Task<IEnumerable<LabDTO>> GetAllLabsAsync(int requesterId, Constant.UserRole requesterRole);
        Task<PagedResult<LabDTO>> GetLabsAsync(QueryParameters queryParams, int requesterId, Constant.UserRole requesterRole);
        Task<LabDTO?> GetLabByIdAsync(int id);
        Task<LabDTO> CreateLabAsync(CreateLabDTO createLabDTO);
        Task<LabDTO?> UpdateLabAsync(UpdateLabDTO updateLabDTO, string name);
        Task<bool> DeleteLabAsync(int id);
        Task<bool> LabExistsAsync(string name);
    }
}
