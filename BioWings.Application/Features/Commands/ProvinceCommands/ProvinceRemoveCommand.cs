using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ProvinceCommands;

public class ProvinceRemoveCommand(int id) : IRequest<ServiceResult>
{
    public int Id { get; set; } = id;
}
