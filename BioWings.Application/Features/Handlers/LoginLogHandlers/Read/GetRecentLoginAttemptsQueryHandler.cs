using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Features.Queries.LoginLogQueries;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;

namespace BioWings.Application.Features.Handlers.LoginLogHandlers.Read;

public class GetRecentLoginAttemptsQueryHandler(ILoginLogService loginLogService) : IRequestHandler<GetRecentLoginAttemptsQuery, ServiceResult<IEnumerable<LoginLogCreateDto>>>
{
    public async Task<ServiceResult<IEnumerable<LoginLogCreateDto>>> Handle(GetRecentLoginAttemptsQuery request, CancellationToken cancellationToken)
    {
        var result = await loginLogService.GetRecentLoginAttemptsAsync(request.Count, cancellationToken);
        return ServiceResult<IEnumerable<LoginLogCreateDto>>.Success(result);
    }
} 