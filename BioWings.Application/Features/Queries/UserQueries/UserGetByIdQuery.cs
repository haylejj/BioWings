using BioWings.Application.Features.Results.UserResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.UserQueries;
public class UserGetByIdQuery(int id) : IRequest<ServiceResult<UserGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
