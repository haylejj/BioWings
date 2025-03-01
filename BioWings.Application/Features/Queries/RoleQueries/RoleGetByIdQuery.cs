using BioWings.Application.Features.Results.RoleResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.RoleQueries;
public class RoleGetByIdQuery(int id) : IRequest<ServiceResult<RoleGetByIdQueryResult>>
{
    public int Id { get; set; } = id;

}
