using BioWings.Application.Features.Results.SubspeciesResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.SubspeciesQueries;
public class SubspeciesGetByIdQuery(int id) : IRequest<ServiceResult<SubspeciesGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
