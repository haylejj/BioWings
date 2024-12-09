using BioWings.Application.Features.Results.ProvinceResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ProvinceQueries;

public class ProvinceGetByIdQuery(int id) : IRequest<ServiceResult<ProvinceGetByIdQueryResult>>
{
    public int Id { get; set; } = id;
}
