namespace BioWings.UI.Areas.Admin.Models.LoginLog;

public class LoginLogViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoginDateTime { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? FailureReason { get; set; }
    public string FormattedDateTime => LoginDateTime.ToString("dd.MM.yyyy HH:mm:ss");
    public string StatusText => IsSuccessful ? "Başarılı" : "Başarısız";
    public string StatusBadge => IsSuccessful ? "success" : "danger";
}

public class LoginLogIndexViewModel
{
    public List<LoginLogViewModel> LoginLogs { get; set; } = new();
    public int TotalCount { get; set; }
    public int SuccessfulCount { get; set; }
    public int FailedCount { get; set; }
}