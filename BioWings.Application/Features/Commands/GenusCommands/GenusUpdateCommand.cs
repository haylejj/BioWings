using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.GenusCommands;

public class GenusUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int FamilyId { get; set; }
}
