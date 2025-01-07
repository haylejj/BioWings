using BioWings.Application.DTOs.ProgressHubDtos;
using BioWings.Application.Services;
using BioWings.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BioWings.Infrastructure.Services;
public class ProgressTracker(IHubContext<ProgressHub> hubContext, ILogger<ProgressTracker> logger) : IProgressTracker
{
    public async Task UpdateProgress(ProgressInfo progress)
    {
        try
        {
            await hubContext.Clients.All.SendAsync("ReceiveProgress", progress);
            logger.LogInformation("Progress update sent: {Progress}%, Message: {Message}", progress.Progress, progress.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send progress update");
        }
    }
}
