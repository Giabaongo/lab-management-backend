using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Interfaces
{
    public interface ISecurityLogService
    {
        Task<IEnumerable<SecurityLogDTO>> GetAllSecurityLogsAsync();
        Task<SecurityLogDTO?> GetSecurityLogByIdAsync(int id);
        Task<SecurityLogDTO> CreateSecurityLogAsync(CreateSecurityLogDTO createSecurityLogDTO);
        Task<SecurityLogDTO?> UpdateSecurityLogAsync(int id, UpdateSecurityLogDTO updateSecurityLogDTO);
        Task<bool> DeleteSecurityLogAsync(int id);
        Task<bool> SecurityLogExistsAsync(int id);
    }
}
