using BioWings.Application.DTOs.ProgressHubDtos;
using BioWings.Application.Services;

namespace BioWings.Infrastructure.Services;
public class ObservationImportProgressTracker(IProgressTracker progressTracker) : IObservationImportProgressTracker
{
    private int _totalRecords;
    private int _currentRecord;
    public void SetTotalRecords(int total)
    {
        _totalRecords = total;
        _currentRecord = 0;
        UpdateProgress();
    }

    public void IncrementProgress(int count = 1)
    {
        _currentRecord += count;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        var progress = _totalRecords > 0 ? _currentRecord * 100 / _totalRecords : 0;
        progressTracker.UpdateProgress(new ProgressInfo
        {
            Progress = progress,
            Message = $"Processing record {_currentRecord} of {_totalRecords}",
            Status = progress == 100 ? "Completed" : "Processing"
        });
    }
}
