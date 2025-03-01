using BioWings.Application.Features.Results.EncryptionResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.EncryptionCommands;
public class DecryptCommand(string code) : IRequest<ServiceResult<DecryptResult>>
{
    public string code { get; set; } = code;
}
