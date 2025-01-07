namespace BioWings.Application.Services;
public interface IObservationImportProgressTracker
{
    void SetTotalRecords(int total);
    void IncrementProgress(int count = 1);
}
