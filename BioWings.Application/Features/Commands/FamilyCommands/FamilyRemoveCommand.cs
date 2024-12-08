using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.FamilyCommands;

public class FamilyRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}