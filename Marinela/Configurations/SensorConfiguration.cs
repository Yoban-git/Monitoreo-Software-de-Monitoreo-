using Marinela.Models.Sensores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marinela.Configurations
{
    public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
    {
        public void Configure(EntityTypeBuilder<Sensor> builder)
        {
            builder.ToTable("Sensores");
            builder.ToTable(tb => tb.UseSqlOutputClause(false));

            builder.HasKey(s => s.IdSensor);
            builder.Property(s => s.Tag).HasMaxLength(100).IsRequired();

            builder.Property(s => s.Creado_En).HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.Modificado_En).HasDefaultValueSql("GETDATE()");

            builder.Property(s => s.Activo).HasDefaultValue(true);

            //se ustiliza HasMany porque hay una relacion uno a Muchos, un sensor tiene muchos datos, y un dato perteen a un sensor
            builder.HasMany(s => s.Datos).WithOne(d => d.Sensor).HasForeignKey(d => d.IdSensor).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
