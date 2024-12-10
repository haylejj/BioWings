using BioWings.Application.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Features.Commands.SpeciesCommands;
public class SpeciesCreateCommand : IRequest<ServiceResult>
{
    public int AuthorityId { get; set; }
    public int GenusId { get; set; }
    public int SpeciesTypeId { get; set; }
    public string ScientificName { get; set; }
    public string Name { get; set; }
    public string EUName { get; set; }
    public string FullName { get; set; }
    public string TurkishName { get; set; }
    public string EnglishName { get; set; }
    public string TurkishNamesTrakel { get; set; }
    public string Trakel { get; set; }
    public string KocakName { get; set; }
    public string HesselbarthName { get; set; }
    public List<IFormFile> FormFiles { get; set; }//IFormFile is used to upload files . its equivalent to Media 
}
