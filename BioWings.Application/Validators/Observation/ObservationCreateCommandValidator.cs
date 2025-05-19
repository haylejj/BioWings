using BioWings.Application.Features.Commands.ObservationCommands;
using FluentValidation;

namespace BioWings.Application.Validators.Observation;
public class ObservationCreateCommandValidator : AbstractValidator<ObservationCreateCommand>
{
    public ObservationCreateCommandValidator()
    {
        RuleFor(x=> x.ScientificName).NotEmpty().NotNull().WithMessage("Scientific name is required");
        RuleFor(x=> x.Name).NotEmpty().NotNull().WithMessage("Name is required");   
        RuleFor(x=> x.ProvinceId).NotEmpty().NotNull().WithMessage("Province is required");
        RuleFor(x => x.Latitude).NotEmpty().NotNull().WithMessage("Latitude is required");
        RuleFor(x => x.Longitude).NotEmpty().NotNull().WithMessage("Longitude is required");
        RuleFor(x => x.ObservationDate).NotEmpty().WithMessage("Observation date is required");
        RuleFor(x => x.NumberSeen).NotEmpty().WithMessage("Number seen is required");
        RuleFor(x => x.NumberSeen).GreaterThan(0).WithMessage("Number seen must be greater than 0");
    }
}
