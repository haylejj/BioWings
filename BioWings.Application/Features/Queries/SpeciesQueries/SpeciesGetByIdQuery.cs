using BioWings.Application.Features.Results.SpeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SpeciesQueries;
public class SpeciesGetByIdQuery(int id) : IRequest<ServiceResult<SpeciesGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
