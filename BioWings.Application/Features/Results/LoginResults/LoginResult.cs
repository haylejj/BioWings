namespace BioWings.Application.Features.Results.LoginResults;
public class LoginResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime Expiration { get; set; }
    public List<string> Roles { get; set; }
}
