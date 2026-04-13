using Marinela.Models.Permisos;
using Marinela.Models.Roles;

namespace Marinela.Models.RolesPermisos
{
    public class RolPermiso
    {
        public int IdRol { get; set; }
        public Rol? Rol { get; set; }
        public int IdPermiso { get; set; }
        public Permiso? Permiso { get; set; }
    }
}