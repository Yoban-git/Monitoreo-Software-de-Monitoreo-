using Marinela.Configurations;
using Marinela.Models.DatosExcel;
using Marinela.Models.DatosSensores;
using Marinela.Models.Permisos;
using Marinela.Models.Roles;
using Marinela.Models.RolesPermisos;
using Marinela.Models.Sensores;
using Marinela.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Marinela.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolPermiso { get; set; }

        //Lecturas
        public DbSet<Sensor> Sensores { get; set; }
        public DbSet<DatoSensor> DatosSensores { get; set; }
        public DbSet<DatoExcel> DatosExcel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
            modelBuilder.ApplyConfiguration(new RolConfiguration());
            modelBuilder.ApplyConfiguration(new PermisoConfiguration());
            modelBuilder.ApplyConfiguration(new RolPermisoConfiguration());

            modelBuilder.ApplyConfiguration(new SensorConfiguration());
            modelBuilder.ApplyConfiguration(new DatoSensorConfiguration());
        }
    }
}
