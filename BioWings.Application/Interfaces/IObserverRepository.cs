using BioWings.Domain.Entities;

namespace BioWings.Application.Interfaces;

public interface IObserverRepository : IGenericRepository<Observer>
{
    Task<Observer?> GetByNameAndSurnameAsync(string name, string surname, CancellationToken cancellationToken = default);
    Task<Observer?> GetByFullNameAsync(string fullName, CancellationToken cancellationToken = default);
}


