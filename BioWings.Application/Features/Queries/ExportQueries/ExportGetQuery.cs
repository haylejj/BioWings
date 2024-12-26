using BioWings.Application.Features.Results.ExportResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ExportQueries;
public class ExportGetQuery : IRequest<ServiceResult<IEnumerable<ExportGetQueryResult>>>
{
}
