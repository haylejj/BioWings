using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.CountryCommands;
public class CountryCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
}
