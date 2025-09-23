using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Login(LoginDTO loginDTO);
    }
}
