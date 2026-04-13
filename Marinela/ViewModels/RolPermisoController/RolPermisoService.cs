using Marinela.Models.Data;
using Marinela.Models.Permisos;
using Marinela.Models.Roles;
using Marinela.Models.RolesPermisos;
using Marinela.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.RolPermisoController
{
    public class ResultadoOperacionRP
    {
        public bool Exito { get; set; }
        public string Error { get; set; } = string.Empty;
    }
    public class RolPermisoService
    {
        private readonly AppDbContext _context;
        // Constructor que recibe el contexto de la base de datos
        public RolPermisoService(AppDbContext context)
        {
            _context = context;
        }
        // Listar todas las relaciones Rol-Permiso
        public async Task<List<RolPermiso>> ObtenerTodasRelacionesAsync()
        {
            try
            {
                var relaciones = await _context.RolPermiso
                    .Include(rp => rp.Rol)       // Incluye datos del Rol
                    .Include(rp => rp.Permiso)   // Incluye datos del Permiso
                    .ToListAsync();

                // Si EF devuelve null (muy raro), inicializamos lista vacía
                if (relaciones == null)
                    return new List<RolPermiso>();

                // Normalizar cada relación para que no se rompa por nulos
                foreach (var rp in relaciones)
                {
                    rp.Rol ??= new Rol
                    {
                        IdRol = 0,
                        NombreRol = string.Empty,
                        Usuarios = new List<Usuario>(),
                        RolPermisos = new List<RolPermiso>()
                    };

                    rp.Permiso ??= new Permiso
                    {
                        IdPermiso = 0,
                        NombrePermiso = string.Empty,
                        RolPermisos = new List<RolPermiso>()
                    };
                }
                return relaciones;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener relaciones Rol-Permiso: {ex.Message}");
                return new List<RolPermiso>(); // nunca null
            }
        }


        // Listar todos los permisos asignados a un rol
        public async Task<List<Permiso>> ObtenerPermisosPorRolAsync(int idRol)
        {
            try
            {
                var permisos = await _context.RolPermiso
                    .Where(rp => rp.IdRol == idRol)
                    .Include(rp => rp.Permiso)
                    .Select(rp => rp.Permiso)
                    .Where(p => p != null)
                    .ToListAsync();

                // Si EF devuelve null (muy raro), inicializamos lista vacía
                if (permisos == null)
                    return new List<Permiso>();

                return permisos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener permisos por rol: {ex.Message}");
                return new List<Permiso>(); // nunca null
            }
        }


        // Asignar un permiso a un rol
        public async Task<ResultadoOperacionRP> AsignarPermisoARolAsync(int idRol, int idPermiso)
        {
            try
            {
                // Validar que el rol exista en la tabla de roles
                var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == idRol);
                if (!rolExiste)
                    return new ResultadoOperacionRP { Exito = false, Error = "El rol no existe." };
                // Validar que el permiso exista en la tabla de permisos
                var permisoExiste = await _context.Permisos.AnyAsync(p => p.IdPermiso == idPermiso);
                if (!permisoExiste)
                    return new ResultadoOperacionRP { Exito = false, Error = "El permiso no existe." };
                // Validar si ya existe la relación
                var existe = await _context.RolPermiso.AnyAsync(rp => rp.IdRol == idRol && rp.IdPermiso == idPermiso);

                if (existe)
                {
                    return new ResultadoOperacionRP
                    {
                        Exito = false,
                        Error = "El permiso ya está asignado a este rol."
                    };
                }

                var rolPermiso = new RolPermiso
                {
                    IdRol = idRol,
                    IdPermiso = idPermiso
                };

                _context.RolPermiso.Add(rolPermiso);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionRP { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionRP { Exito = false, Error = ex.Message };
            }
        }

        // Actualizar el permiso de un rol
        public async Task<ResultadoOperacionRP> ActualizarPermisoDeRolAsync(int idRol, int idPermisoActual, int idPermisoNuevo)
        {
            try
            {
                // Validación: ids válidos
                if (idRol <= 0 || idPermisoActual <= 0 || idPermisoNuevo <= 0)
                {
                    return new ResultadoOperacionRP { Exito = false, Error = "Los identificadores deben ser mayores a 0." };
                }

                // Validar que el rol exista
                var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == idRol);
                if (!rolExiste)
                {
                    return new ResultadoOperacionRP { Exito = false, Error = "El rol no existe." };
                }

                // Validar que el permiso nuevo exista
                var permisoExiste = await _context.Permisos.AnyAsync(p => p.IdPermiso == idPermisoNuevo);
                if (!permisoExiste)
                {
                    return new ResultadoOperacionRP { Exito = false, Error = "El permiso nuevo no existe." };
                }

                // Buscar la relación existente
                var rolPermiso = await _context.RolPermiso
                    .FirstOrDefaultAsync(rp => rp.IdRol == idRol && rp.IdPermiso == idPermisoActual);

                if (rolPermiso == null)
                {
                    return new ResultadoOperacionRP { Exito = false, Error = "La relación Rol-Permiso no existe." };
                }

                // Validar que el nuevo permiso no esté ya asignado al rol
                var existe = await _context.RolPermiso.AnyAsync(rp => rp.IdRol == idRol && rp.IdPermiso == idPermisoNuevo);
                if (existe)
                {
                    return new ResultadoOperacionRP { Exito = false, Error = "El rol ya tiene asignado este nuevo permiso." };
                }

                // Actualizar el permiso
                _context.RolPermiso.Remove(rolPermiso);
                _context.RolPermiso.Add(new RolPermiso { IdRol = idRol, IdPermiso = idPermisoNuevo });
                await _context.SaveChangesAsync();


                return new ResultadoOperacionRP { Exito = true, Error = null! };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionRP
                {
                    Exito = false,
                    Error = $"Error inesperado: {ex.Message}"
                };
            }
        }


        // Eliminar un permiso de un rol
        public async Task<ResultadoOperacionRP> EliminarPermisoDeRolAsync(int idRol, int idPermiso)
        {
            try
            {
                // Validar que la relación exista antes de intentar eliminarla
                var rolPermiso = await _context.RolPermiso
                //FirstOrDefaultAsync devuelve el primer elemento que cumple la condición
                    .FirstOrDefaultAsync(rp => rp.IdRol == idRol && rp.IdPermiso == idPermiso);
                // Si no existe, devolvemos un error específico
                if (rolPermiso == null)
                {
                    return new ResultadoOperacionRP { Exito = false, Error = "La relación Rol-Permiso no existe." };
                }
                // Si existe, procedemos a eliminarla
                _context.RolPermiso.Remove(rolPermiso);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionRP { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionRP
                {
                    Exito = false,
                    Error = $"Error inesperado al eliminar permiso: {ex.Message}"
                };
            }
        }
    }
}
