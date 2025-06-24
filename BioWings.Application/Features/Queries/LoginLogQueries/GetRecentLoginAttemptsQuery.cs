using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.LoginLogQueries;

public class GetRecentLoginAttemptsQuery : IRequest<ServiceResult<IEnumerable<LoginLogCreateDto>>>
{
    public int Count { get; set; } = 100;
} 