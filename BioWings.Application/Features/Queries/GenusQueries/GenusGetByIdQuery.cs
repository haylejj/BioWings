using BioWings.Application.Features.Results.GenusResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.GenusQueries;
public class GenusGetByIdQuery(int id) : IRequest<ServiceResult<GenusGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
