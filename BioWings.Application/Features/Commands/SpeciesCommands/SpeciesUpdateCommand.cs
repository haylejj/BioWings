using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.SpeciesCommands;

public class SpeciesUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public int? AuthorityYear { get; set; }
    public string? AuthorityName { get; set; }
    //public int? FamilyId { get; set; }
    public int? GenusId { get; set; }
    public string? ScientificName { get; set; }
    public string? Name { get; set; }
    public string? EUName { get; set; }
    public string? FullName { get; set; }
    public string? TurkishName { get; set; }
    public string? EnglishName { get; set; }
    public string? TurkishNamesTrakel { get; set; }
    public string? Trakel { get; set; }
    public string? KocakName { get; set; }
    public string? HesselbarthName { get; set; }
}
