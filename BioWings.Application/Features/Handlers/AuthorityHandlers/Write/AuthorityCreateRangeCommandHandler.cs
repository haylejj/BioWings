using BioWings.Application.Features.Commands.AuthorityCommands;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Write;

public class AuthorityCreateRangeCommandHandler(IAuthorityRepository repository, ILogger<AuthorityCreateRangeCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<AuthorityCreateRangeCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(AuthorityCreateRangeCommand request, CancellationToken cancellationToken)
    {
        if (request.AuthorityCreateDtos == null || !request.AuthorityCreateDtos.Any())
        {
            logger.LogWarning("AuthorityCreateRangeCommand is null");
            return ServiceResult.Error("AuthorityCreateRangeCommand is null", System.Net.HttpStatusCode.BadRequest);
        }
        var authorities = request.AuthorityCreateDtos.Select(dto => new Authority
        {
            Name = dto.Name,
            Year = dto.Year
        });
        await repository.AddRangeAsync(authorities, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Authorities created");
        return ServiceResult.SuccessAsCreated("api/Authorities/Range");
    }
}
