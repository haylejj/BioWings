namespace BioWings.Domain.Entities;
public class User : BaseEntity
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } // Hash'lenmiş şekilde saklanacak
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    // Token ile ilgili tutulması gereken bilgiler
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireDate { get; set; }

    // Şifre sıfırlama için
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpireDate { get; set; }

    // Email doğrulama için
    public bool EmailConfirmed { get; set; }
    public string? EmailConfirmationToken { get; set; }
    public DateTime? EmailConfirmationTokenExpireDate { get; set; }
    // Navigation property for roles
    public virtual ICollection<UserRole> UserRoles { get; set; }
}
