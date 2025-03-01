namespace BioWings.UI.Areas.Admin.Models.Role;

public class RoleViewModel
{
    public RoleCreateViewModel? CreateViewModel { get; set; }
    public RoleUpdateViewModel? UpdateViewModel { get; set; }
    public List<RoleGetViewModel> GetViewModels { get; set; } = new List<RoleGetViewModel>();
}
