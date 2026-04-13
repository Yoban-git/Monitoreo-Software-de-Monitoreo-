using Marinela.Models.DatosSensores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Marinela.Configurations
{
    public class DatoSensorConfiguration : IEntityTypeConfiguration<DatoSensor>
    {
        public void Configure(EntityTypeBuilder<DatoSensor> builder)
        {
            builder.ToTable("DatosSensores");
            builder.ToTable(tb => tb.UseSqlOutputClause(false));

            builder.HasKey(d => d.IdDato);

            builder.Property(d => d.Dato).IsRequired();

            builder.Property(d => d.Creado_En).HasDefaultValueSql("GETDATE()");
        }
    }
}
