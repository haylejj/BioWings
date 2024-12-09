using BioWings.Application.Features.Commands.AuthorityCommands;
using BioWings.Application.Results;
using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BioWings.Application.Features.Handlers.AuthorityHandlers.Write;
public class AuthorityCreateCommandHandler(IAuthorityRepository repository, ILogger<AuthorityCreateCommandHandler> logger, IUnitOfWork unitOfWork) : IRequestHandler<AuthorityCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(AuthorityCreateCommand request, CancellationToken cancellationToken)
    {
        if (request==null)
        {
            logger.LogWarning("AuthorityCreateCommand is null");
            return ServiceResult.Error("AuthorityCreateCommand is null", System.Net.HttpStatusCode.BadRequest);
        }
        var authority = new Authority
        {
            Name = request.Name,
            Year = request.Year
        };
        await repository.AddAsync(authority, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Authority created Id:{0}", authority.Id);
        return ServiceResult.SuccessAsCreated("api/Authorities/"+authority.Id);
    }
}
