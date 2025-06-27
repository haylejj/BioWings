namespace BioWings.Application.DTOs.PasswordDtos;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
} 