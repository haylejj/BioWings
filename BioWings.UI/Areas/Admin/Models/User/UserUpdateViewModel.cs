namespace BioWings.UI.Areas.Admin.Models.User;

public class UserUpdateViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public int CountryId { get; set; }
}
