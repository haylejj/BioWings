using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationQueries;
public class ObservationGetFilteredQuery : IRequest<ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>>
{
    public List<string> ColumnNames { get; set; }
    public List<string> ColumnValues { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}