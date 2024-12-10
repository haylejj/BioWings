using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesQueries;
public class SpeciesGetPagedQuery : IRequest<ServiceResult<PaginatedList<SpeciesGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
