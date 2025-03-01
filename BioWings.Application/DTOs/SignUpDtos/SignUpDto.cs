namespace BioWings.Application.DTOs.SignUpDtos;
public class SignUpDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int CountryId { get; set; }
}
