using BioWings.Application.DTOs.ProgressHubDtos;

namespace BioWings.Application.Services;
public interface IProgressTracker
{
    Task UpdateProgress(ProgressInfo progress);
}
