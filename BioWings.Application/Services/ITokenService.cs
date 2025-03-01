using BioWings.Application.DTOs.JwtDtos;
using BioWings.Application.DTOs.UserRoleDtos;
using BioWings.Domain.Entities;
using System.Security.Claims;

namespace BioWings.Application.Services;
public interface ITokenService
{
    TokenResponse CreateToken(User user);
    TokenResponse CreateToken(User user, List<UserRoleGetByUserIdDto> roles);
    string CreateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
