using BioWings.Application.Features.Results.CountryResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.CountryQueries;

public class CountryGetByIdQuery(int id) : IRequest<ServiceResult<CountryGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
