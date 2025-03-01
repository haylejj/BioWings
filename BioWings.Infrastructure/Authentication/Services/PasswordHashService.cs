using BioWings.Application.Services;

namespace BioWings.Infrastructure.Authentication.Services;
public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    public bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}
