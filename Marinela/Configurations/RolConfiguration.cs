using Marinela.Models.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marinela.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.IdRol);
            builder.Property(r => r.NombreRol).HasMaxLength(100).IsRequired();
            builder.HasIndex(r => r.NombreRol).IsUnique();
        }
    }
}
