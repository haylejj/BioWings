using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Commands.LocationCommands;

public class LocationUpdateCommand : IRequest<ServiceResult>
{
    public int Id { get; set; }
    public int ProvinceId { get; set; }
    public string Name { get; set; }
    public string SquareRef { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal Altitude1 { get; set; }
    public decimal Altitude2 { get; set; }
    public decimal XCoord { get; set; }
    public decimal YCoord { get; set; }
    public string UtmReference { get; set; }
    public string Description { get; set; }

}
