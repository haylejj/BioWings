using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.GenusQueries;
public class GenusGetCountQuery : IRequest<ServiceResult<GenusGetCountQueryResult>>
{
}
