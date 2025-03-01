namespace BioWings.Application.Features.Results.UserResults;
public class UserGetQueryResult
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string CountryName { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
