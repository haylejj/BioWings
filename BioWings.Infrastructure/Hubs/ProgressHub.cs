using BioWings.Application.DTOs.ProgressHubDtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
namespace BioWings.Infrastructure.Hubs;
public class ProgressHub(ILogger<ProgressHub> logger) : Hub
{
    public async Task UpdateProgress(ProgressInfo progress)
    {
        await Clients.All.SendAsync("ReceiveProgress", progress);
    }
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
