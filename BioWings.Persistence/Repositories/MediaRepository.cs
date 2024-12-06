using BioWings.Domain.Entities;
using BioWings.Domain.Interfaces;
using BioWings.Persistence.Context;

namespace BioWings.Persistence.Repositories;

public class MediaRepository(AppDbContext dbContext) : GenericRepository<Media>(dbContext), IMediaRepository
{
}

