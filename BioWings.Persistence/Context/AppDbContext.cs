using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BioWings.Persistence.Context;
public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    public DbSet<Species> Species { get; set; }
    public DbSet<Genus> Genera { get; set; }
    public DbSet<Subspecies> Subspecies { get; set; }
    public DbSet<Observation> Observations { get; set; }
    public DbSet<Authority> Authorities { get; set; }
    public DbSet<Family> Families { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Observer> Observers { get; set; }
    public DbSet<Province> Provinces { get; set; }
}

