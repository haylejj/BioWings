using System.ComponentModel.DataAnnotations;

namespace BioWings.UI.ViewModels.EmailViewModels;

public class ForgetPasswordViewModel
{
    [Required(ErrorMessage = "E-posta adresi gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; }
}
