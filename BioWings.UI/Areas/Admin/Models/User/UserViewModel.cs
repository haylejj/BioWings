namespace BioWings.UI.Areas.Admin.Models.User;

public class UserViewModel
{
    public List<UserGetViewModel>? GetViewModels { get; set; } = new List<UserGetViewModel>();
    public UserUpdateViewModel? UpdateViewModel { get; set; }
    public UserCreateViewModel? CreateViewModel { get; set; }
}
