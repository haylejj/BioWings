using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesTypeQueries;
public class SpeciesTypeGetPagedQuery : IRequest<ServiceResult<PaginatedList<SpeciesTypeGetPagedQueryResult>>>
{
    public int PageNumber { get; set; }  // kullanıcı girecek
    public int PageSize { get; set; }    // kullanıcı girecek
}
