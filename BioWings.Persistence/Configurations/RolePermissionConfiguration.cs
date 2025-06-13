using BioWings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BioWings.Persistence.Configurations
{
    /// <summary>
    /// RolePermission entity için EF Core konfigürasyonu
    /// </summary>
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // Tablo adı
            builder.ToTable("RolePermissions");

            // Composite Primary Key
            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Foreign Key - Role
            builder.HasOne(rp => rp.Role)
                   .WithMany(r => r.RolePermissions)
                   .HasForeignKey(rp => rp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Foreign Key - Permission
            builder.HasOne(rp => rp.Permission)
                   .WithMany(p => p.RolePermissions)
                   .HasForeignKey(rp => rp.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Performance için index'ler
            builder.HasIndex(rp => rp.RoleId)
                   .HasDatabaseName("IX_RolePermissions_RoleId");

            builder.HasIndex(rp => rp.PermissionId)
                   .HasDatabaseName("IX_RolePermissions_PermissionId");
        }
    }
} 