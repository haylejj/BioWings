namespace BioWings.Application.DTOs.LoginLogDtos;

public class LoginLogCreateDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoginDateTime { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
}