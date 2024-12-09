using BioWings.Application.Features.Commands.AuthorityCommands;
using BioWings.Application.Results;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Write;

public class AuthorityUpdateCommandHandler(IAuthorityRepository repository, ILogger<AuthorityUpdateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<AuthorityUpdateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(AuthorityUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            logger.LogWarning("AuthorityUpdateCommand is null");
            return ServiceResult.Error("AuthorityUpdateCommand is null", System.Net.HttpStatusCode.BadRequest);
        }
        var authority = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (authority == null)
        {
            logger.LogWarning("Authority not found with ID: {AuthorityId}", request.Id);
            return ServiceResult.Error("Authority not found", System.Net.HttpStatusCode.NotFound);
        }
        authority.Name = request.Name;
        authority.Year = request.Year;
        repository.Update(authority);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Authority updated Id:{0}", authority.Id);
        return ServiceResult.Success(System.Net.HttpStatusCode.NoContent);
    }
}
