using BioWings.Application.DTOs.ExportDtos;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.ExportCommands;
public class ExportCreateCommand : IRequest<ServiceResult<byte[]>>
{
    public List<ExpertColumnInfo> Columns { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? RecordLimit { get; set; }
    public bool ExportAllDates { get; set; }
    public bool ExportAllRecords { get; set; }
}
