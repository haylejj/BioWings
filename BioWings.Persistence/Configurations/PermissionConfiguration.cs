using BioWings.Domain.Entities;
using BioWings.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations
{
    /// <summary>
    /// Permission entity'si için EF Core configuration
    /// </summary>
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            // Primary Key
            builder.HasKey(x => x.Id);

            // Table name
            builder.ToTable("Permissions");

            // Required fields with max length constraints
            builder.Property(x => x.ControllerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ActionName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Definition)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.HttpType)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.MenuName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.AreaName)
                .HasMaxLength(100)
                .HasDefaultValue("");

            builder.Property(x => x.PermissionCode)
                .IsRequired()
                .HasMaxLength(250);

            // Enum configuration
            builder.Property(x => x.ActionType)
                .IsRequired()
                .HasConversion<string>(); // Enum'u string olarak sakla

            // Unique index for PermissionCode
            builder.HasIndex(x => x.PermissionCode)
                .IsUnique()
                .HasDatabaseName("IX_Permissions_PermissionCode");

            // Composite index for better query performance
            builder.HasIndex(x => new { x.ControllerName, x.ActionName, x.AreaName })
                .HasDatabaseName("IX_Permissions_Controller_Action_Area");

            // Index for ActionType
            builder.HasIndex(x => x.ActionType)
                .HasDatabaseName("IX_Permissions_ActionType");
        }
    }
} 