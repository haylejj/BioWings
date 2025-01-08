namespace BioWings.UI.ViewModels.ObserverViewModels;

public class ObserverGetObservationsViewModel
{
    public int Id { get; set; }
    public string? ScientificName { get; set; }
    public string? SpeciesName { get; set; }
    public string? KocakName { get; set; }
    public string GenusName { get; set; }
    public string FamilyName { get; set; }
    public string ProvinceName { get; set; }
    public DateTime ObservationDate { get; set; }
    public int NumberSeen { get; set; }
    public string Source { get; set; }
}
