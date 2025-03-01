namespace BioWings.UI.ViewModels.EmailViewModels;

public class EmailConfirmViewModel
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; }
    public string Email { get; set; }
    public bool ShowResendButton { get; set; } = false;
}
