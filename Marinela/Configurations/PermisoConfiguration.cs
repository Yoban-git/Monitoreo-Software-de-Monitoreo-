using Marinela.Models.Permisos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marinela.Configurations
{
    public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
    {
        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.ToTable("Permisos");
            builder.HasKey(p => p.IdPermiso);
            builder.Property(p => p.NombrePermiso).HasMaxLength(50).IsRequired();

            builder.HasIndex(p => p.NombrePermiso).IsUnique();
        }
    }
}
