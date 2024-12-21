using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class ObserverConfiguration : IEntityTypeConfiguration<Observer>
{
    public void Configure(EntityTypeBuilder<Observer> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(40);
        builder.Property(x => x.Surname)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(40);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(70);

        builder.HasMany(x => x.Observations)
            .WithOne(x => x.Observer)
            .HasForeignKey(x => x.ObserverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.FullName);
    }
}

