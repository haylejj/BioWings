using BioWings.Application.DTOs.LoginDtos;

namespace BioWings.Application.Services;
public interface ILoginService
{
    Task<bool> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
}
