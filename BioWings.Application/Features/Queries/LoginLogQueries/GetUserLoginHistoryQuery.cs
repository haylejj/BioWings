using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.LoginLogQueries;

public class GetUserLoginHistoryQuery : IRequest<ServiceResult<IEnumerable<LoginLogCreateDto>>>
{
    public int UserId { get; set; }
}