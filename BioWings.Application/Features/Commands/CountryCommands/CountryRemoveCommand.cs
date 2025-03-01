using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.CountryCommands;
public class CountryRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
