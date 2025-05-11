using BioWings.Application.Features.Commands.SpeciesCommands;
using FluentValidation;

namespace BioWings.Application.Validators.Species;

public class SpeciesUpdateCommandValidator : AbstractValidator<SpeciesUpdateCommand>
{
    public SpeciesUpdateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage("Species name is required.")
            .MaximumLength(100)
            .WithMessage("Species name must not exceed 100 characters.");
    }
}
