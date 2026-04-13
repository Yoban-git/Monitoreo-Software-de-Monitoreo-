using Marinela.Models.RolesPermisos;
using System.ComponentModel.DataAnnotations;

namespace Marinela.Models.Permisos
{
    public class Permiso
    {
        [Key]
        public int IdPermiso { get; set; }

        [Required(ErrorMessage = "El nombre del permiso es obligatorio")]
        public string NombrePermiso { get; set; } = string.Empty; // inicializado

        // Relación muchos a muchos con roles
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    }
}
