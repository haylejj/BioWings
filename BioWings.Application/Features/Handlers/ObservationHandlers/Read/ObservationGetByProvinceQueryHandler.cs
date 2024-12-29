using BioWings.Application.Features.Queries.ObservationQueries;
using BioWings.Application.Features.Results.ObservationResults;
using BioWings.Application.Interfaces;
using BioWings.Application.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BioWings.Application.Features.Handlers.ObservationHandlers.Read;
public class ObservationGetByProvinceQueryHandler(IObservationRepository observationRepository, ILogger<ObservationGetByProvinceQueryHandler> logger) : IRequestHandler<ObservationGetByProvinceQuery, ServiceResult<PaginatedList<ObservationGetByProvinceQueryResult>>>
{
    public async Task<ServiceResult<PaginatedList<ObservationGetByProvinceQueryResult>>> Handle(ObservationGetByProvinceQuery request, CancellationToken cancellationToken)
    {
        request.PageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        request.PageSize = request.PageSize <= 0 ? 25 : Math.Min(request.PageSize, 50);

        var query = observationRepository.GetAllAsQueryable().AsNoTracking().Include(x => x.Location).ThenInclude(x => x.Province).AsQueryable();
        if (request.ProvinceCode <= 0)
        {
            logger.LogInformation("Province code is not valid");
            return ServiceResult<PaginatedList<ObservationGetByProvinceQueryResult>>.Error("Province code is not valid", HttpStatusCode.BadRequest);
        }
        query = query.Where(x => x.Location.ProvinceId == request.ProvinceCode)
            .Include(x => x.Species).ThenInclude(x => x.Genus).ThenInclude(x => x.Family)
            .Include(x => x.Species.Authority)
            .Include(x => x.Observer);
        var totalCount = query.Count();

        query= query.OrderByDescending(x => x.ObservationDate).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);

        var result = await query.Select(x => new ObservationGetByProvinceQueryResult
        {
            AuthorityName = x.Species.Authority.Name,
            AuthorityYear = x.Species.Authority.Year,
            GenusName = x.Species.Genus.Name,
            FamilyName = x.Species.Genus.Family.Name,
            SpeciesName = x.Species.Name,
            ScientificName = x.Species.ScientificName,
            Trakel = x.Species.Trakel,
            KocakName = x.Species.KocakName,
            Latitude = x.Location.Latitude,
            Longitude = x.Location.Longitude,
            NumberSeen = x.NumberSeen,
            ObservationDate=x.ObservationDate,
            Id=x.Id
        }).ToListAsync();

        var paginatedResult = new PaginatedList<ObservationGetByProvinceQueryResult>(
            result,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        logger.LogInformation("Observations are fetched successfully by province code");
        return ServiceResult<PaginatedList<ObservationGetByProvinceQueryResult>>.Success(paginatedResult);
    }
}
