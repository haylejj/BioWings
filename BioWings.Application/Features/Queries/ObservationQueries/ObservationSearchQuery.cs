using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationQueries;
public class ObservationSearchQuery:IRequest<ServiceResult<PaginatedList<ObservationGetPagedQueryResult>>>
{
    public string? SearchTerm { get; set; }  
    public int PageNumber { get; set; } = 1; 
    public int PageSize { get; set; } = 25;  
}
