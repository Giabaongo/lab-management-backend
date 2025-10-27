using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Login(LoginDTO loginDTO);
    }
}
