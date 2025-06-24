namespace BioWings.Domain.Entities;

public class LoginLog : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string IpAddress { get; set; }
    public DateTime LoginDateTime { get; set; }
    public string UserAgent { get; set; }
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    
    // Navigation property
    public virtual User User { get; set; }
} 