using BioWings.Application.Features.Results.UserRoleResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.UserRoleQueries;
public class UserRoleGetByUserIdQuery(int id) : IRequest<ServiceResult<UserRoleGetByUserIdResult>>
{
    public int Id { get; set; } = id;
}
