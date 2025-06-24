using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations;

public class LoginLogConfiguration : IEntityTypeConfiguration<LoginLog>
{
    public void Configure(EntityTypeBuilder<LoginLog> builder)
    {
        builder.ToTable("LoginLogs");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.UserId)
            .IsRequired();
            
        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(x => x.IpAddress)
            .IsRequired()
            .HasMaxLength(45); // IPv6 için yeterli
            
        builder.Property(x => x.LoginDateTime)
            .IsRequired();
            
        builder.Property(x => x.UserAgent)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(x => x.IsSuccessful)
            .IsRequired();
            
        builder.Property(x => x.FailureReason)
            .HasMaxLength(200);

        // Foreign key relationship
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for better query performance
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.LoginDateTime);
        builder.HasIndex(x => x.IsSuccessful);
    }
} 