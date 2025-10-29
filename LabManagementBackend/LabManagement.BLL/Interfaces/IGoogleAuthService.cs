using LabManagement.BLL.DTOs;

namespace LabManagement.BLL.Interfaces;

public interface IGoogleAuthService
{
    Task<AuthResponseDTO> LoginWithGoogleAsync(GoogleLoginDTO googleLogin);
}
