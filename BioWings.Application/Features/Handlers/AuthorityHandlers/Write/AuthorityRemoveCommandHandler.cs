using BioWings.Application.Features.Commands.AuthorityCommands;
using BioWings.Application.Results;
using BioWings.Application.Services;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Write;

public class AuthorityRemoveCommandHandler(IAuthorityRepository repository, ILogger<AuthorityRemoveCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<AuthorityRemoveCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(AuthorityRemoveCommand request, CancellationToken cancellationToken)
    {
        var authority = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (authority == null)
        {
            logger.LogWarning("Authority not found with ID: {AuthorityId}", request.Id);
            return ServiceResult.Error("Authority not found", HttpStatusCode.NotFound);
        }
        repository.Remove(authority);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Authority removed successfully with ID: {AuthorityId}", authority.Id);
        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}
