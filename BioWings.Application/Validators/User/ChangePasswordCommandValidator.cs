using BioWings.Application.Features.Commands.UserCommands;
using FluentValidation;
using System.Text.RegularExpressions;

namespace BioWings.Application.Validators.User;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("Kullanıcı ID geçerli olmalıdır");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Mevcut şifre gereklidir")
            .NotNull()
            .WithMessage("Mevcut şifre gereklidir");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Yeni şifre gereklidir")
            .NotNull()
            .WithMessage("Yeni şifre gereklidir")
            .MinimumLength(8)
            .WithMessage("Şifre en az 8 karakter olmalıdır")
            .MaximumLength(100)
            .WithMessage("Şifre en fazla 100 karakter olabilir")
            .Must(BeAValidPassword)
            .WithMessage("Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Şifre onayı gereklidir")
            .NotNull()
            .WithMessage("Şifre onayı gereklidir")
            .Equal(x => x.NewPassword)
            .WithMessage("Şifreler eşleşmiyor");
    }

    private static bool BeAValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // At least one uppercase letter
        if (!Regex.IsMatch(password, @"[A-Z]"))
            return false;

        if (!Regex.IsMatch(password, @"[a-z]"))
            return false;

        if (!Regex.IsMatch(password, @"[0-9]"))
            return false;

        if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
            return false;

        return true;
    }
} 