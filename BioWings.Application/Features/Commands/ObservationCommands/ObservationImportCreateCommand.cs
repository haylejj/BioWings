using BioWings.Application.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Features.Commands.ObservationCommands;
public class ObservationImportCreateCommand : IRequest<ServiceResult>
{
    public IFormFile File { get; set; }
}
