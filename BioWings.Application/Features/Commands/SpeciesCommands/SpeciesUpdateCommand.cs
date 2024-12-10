using BioWings.Application.Results;
using BioWings.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BioWings.Application.Features.Commands.SpeciesCommands;

public class SpeciesUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
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
    public List<IFormFile> NewFiles { get; set; }//IFormFile is used to upload files . its equivalent to new file for Media
    public string DeletedFilesIds { get; set; }
    public List<Media> Medias { get; set; }
}
