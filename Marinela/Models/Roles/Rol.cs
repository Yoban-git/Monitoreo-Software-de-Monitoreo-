using Marinela.Models.RolesPermisos;
using Marinela.Models.Usuarios;
using System.ComponentModel.DataAnnotations;

namespace Marinela.Models.Roles
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        public string NombreRol { get; set; } = string.Empty; // inicializado

        // Relación con usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        // Relación con permisos
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    }
}
