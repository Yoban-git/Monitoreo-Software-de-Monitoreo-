using Marinela.Models.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marinela.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.ToTable(tb => tb.UseSqlOutputClause(false));
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            builder.Property(u => u.UserName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Creado_En).HasDefaultValueSql("GETDATE()");

            builder.Property(u => u.Modificado_En).HasDefaultValueSql("GETDATE()");
            builder.Property(u => u.Activo).HasDefaultValue(true);

            // Relación con Rol
            builder.HasOne(u => u.Rol).WithMany(r => r.Usuarios).HasForeignKey(u => u.IdRol);
        }
    }
}
