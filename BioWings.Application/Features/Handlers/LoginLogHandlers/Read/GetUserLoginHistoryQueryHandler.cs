using BioWings.Application.DTOs.LoginLogDtos;
using BioWings.Application.Features.Queries.LoginLogQueries;
using BioWings.Application.Results;
using BioWings.Application.Services;
using MediatR;

namespace BioWings.Application.Features.Handlers.LoginLogHandlers.Read;

public class GetUserLoginHistoryQueryHandler(ILoginLogService loginLogService) : IRequestHandler<GetUserLoginHistoryQuery, ServiceResult<IEnumerable<LoginLogCreateDto>>>
{
    public async Task<ServiceResult<IEnumerable<LoginLogCreateDto>>> Handle(GetUserLoginHistoryQuery request, CancellationToken cancellationToken)
    {
        var result = await loginLogService.GetUserLoginHistoryAsync(request.UserId, cancellationToken);
        return ServiceResult<IEnumerable<LoginLogCreateDto>>.Success(result);
    }
} 