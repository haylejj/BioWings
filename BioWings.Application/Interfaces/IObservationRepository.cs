using BioWings.Application.DTOs.ExportDtos;
using BioWings.Application.DTOs.StatisticsDtos;
using BioWings.Domain.Entities;
using System.Linq.Expressions;

namespace BioWings.Application.Interfaces;

public interface IObservationRepository : IGenericRepository<Observation>
{
    Task<Observation?> FirstOrDefaultAsync(Expression<Func<Observation, bool>> predicate, CancellationToken cancellationToken = default);
    Task<Observation?> GetByIdWithAllNavigationsAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Observation>> GetAllDuplicateObservationsAsync(CancellationToken cancellationToken = default);
    Task RemoveDuplicatesAsync(List<int> batchIds, CancellationToken cancellationToken = default);
    Task<List<int>> GetAllDuplicateObservationIdsAsync(CancellationToken cancellationToken = default);
    Task BulkInsertObservationsAsync(IList<Observation> observations, CancellationToken cancellationToken = default);
    Task RemoveDuplicatesWithProcedureAsync(string procedureName, CancellationToken cancellationToken = default);
    Task RemoveDuplicateObservationsWithEfCoreAsync(CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    IQueryable<Observation> GetObservationsForExporting(DateTime? startDate, DateTime? endDate, int? recordLimit, bool exportAllDates, bool exportAllRecords, List<ExpertColumnInfo> columns);
    IQueryable<Observation> GetObservationsByProvinceIdForExporting(int provinceId);
    Task<List<ProvinceStatDto>> GetTopProvincesAsync(CancellationToken cancellationToken = default);
}


