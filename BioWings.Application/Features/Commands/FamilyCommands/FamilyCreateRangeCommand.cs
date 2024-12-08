using BioWings.Application.DTOs.Family;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.FamilyCommands;
public class FamilyCreateRangeCommand : IRequest<ServiceResult>
{
    public IEnumerable<FamilyCreateDto> Families { get; set; }
}
