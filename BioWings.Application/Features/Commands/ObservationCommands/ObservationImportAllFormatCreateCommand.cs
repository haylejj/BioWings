using BioWings.Application.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Features.Commands.ObservationCommands;
public class ObservationImportAllFormatCreateCommand : IRequest<ServiceResult>
{
    public IFormFile File { get; set; }
}
