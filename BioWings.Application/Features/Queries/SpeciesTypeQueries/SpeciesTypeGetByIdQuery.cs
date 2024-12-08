using BioWings.Application.Features.Results.SpeciesTypeResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesTypeQueries;
public class SpeciesTypeGetByIdQuery(int id) : IRequest<ServiceResult<SpeciesTypeGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
