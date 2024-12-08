using BioWings.Application.Features.Results.FamilyResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.FamilyQueries;
public class FamilyGetByIdQuery(int id) : IRequest<ServiceResult<FamilyGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
