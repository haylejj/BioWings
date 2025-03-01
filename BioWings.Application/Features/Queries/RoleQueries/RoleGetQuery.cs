using BioWings.Application.Features.Results.RoleResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.RoleQueries;
public class RoleGetQuery : IRequest<ServiceResult<List<RoleGetQueryResult>>>
{
}
