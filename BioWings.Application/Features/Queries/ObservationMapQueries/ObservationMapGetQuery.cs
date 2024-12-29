using BioWings.Application.Features.Results.ObservationMapResults;
using BioWings.Application.Results;
using MediatR;

namespace BioWings.Application.Features.Queries.ObservationMapQueries;
public class ObservationMapGetQuery : IRequest<ServiceResult<List<ObservationMapGetQueryResult>>>
{
    public double MinLat { get; set; }
    public double MaxLat { get; set; }
    public double MinLng { get; set; }
    public double MaxLng { get; set; }
    public int? ZoomLevel { get; set; }
}
