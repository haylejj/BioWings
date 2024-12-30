using BioWings.Application.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Features.Commands.SpeciesCommands;
public class SpeciesImportCreateCommand : IRequest<ServiceResult>
{
    public IFormFile File { get; set; }
}
