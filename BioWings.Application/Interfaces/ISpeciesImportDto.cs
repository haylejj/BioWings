namespace BioWings.Application.Interfaces;

public interface ISpeciesImportDto
{
    string? AuthorityName { get; }
    int AuthorityYear { get; }
    string? GenusName { get; }
    string? ScientificName { get; }
    string? SpeciesName { get; }
    string? EUName { get; }
    string? FullName { get; }
    string? TurkishName { get; }
    string? EnglishName { get; }
    string? TurkishNamesTrakel { get; }
    string? Trakel { get; }
    string? KocakName { get; }
    string? HesselbarthName { get; }
}
