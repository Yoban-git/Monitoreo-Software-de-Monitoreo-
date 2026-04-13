using Marinela.Models.Data;
using Marinela.Models.Permisos;
using Marinela.Models.Roles;
using Marinela.Models.RolesPermisos;
using Marinela.ViewModels.PermisoController;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.RolController
{
    public class ResultadoOperacionR
    {
        public bool Exito { get; set; }
        public string Error { get; set; } = string.Empty;
    }
    public class RolService
    {
        private readonly AppDbContext _context;
        // Constructor que recibe el contexto de la base de datos
        public RolService(AppDbContext context)
        {
            _context = context;
        }

        //listar datos de roles
        public async Task<List<Rol>> ObtenerTodosRolesAsync()
        {
            try
            {
                // Incluimos la relación de permisos para cada rol
                // ToListAsync devuelve una lista de roles con sus permisos asociados, o una lista vacía si no hay roles.
                return await _context.Roles.ToListAsync() ?? new List<Rol>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Roles: {ex.Message}");
                return new List<Rol>();
            }
        }

        // Obtener un rol por Id
        public async Task<Rol> ObtenerRolPorIdAsync(int idRol)
        {
            try
            {
                if (idRol <= 0)
                {
                    // Id inválido, devolvemos un objeto seguro
                    return new Rol
                    {
                        IdRol = 0,
                        NombreRol = string.Empty,
                        RolPermisos = new List<RolPermiso>()
                    };
                }

                var rol = await _context.Roles
                    .Include(r => r.RolPermisos)
                        .ThenInclude(rp => rp.Permiso) // opcional: cargar permisos asociados
                    .FirstOrDefaultAsync(r => r.IdRol == idRol);

                if (rol == null)
                {
                    // Si no existe, devolvemos un objeto seguro
                    return new Rol
                    {
                        IdRol = 0,
                        NombreRol = string.Empty,
                        RolPermisos = new List<RolPermiso>()
                    };
                }

                // Normalizar propiedades para evitar nulos
                rol.NombreRol ??= string.Empty;
                rol.RolPermisos ??= new List<RolPermiso>();

                // Normalizar cada permiso dentro de la relación
                foreach (var rp in rol.RolPermisos)
                {
                    rp.Permiso ??= new Permiso
                    {
                        IdPermiso = 0,
                        NombrePermiso = string.Empty,
                        RolPermisos = new List<RolPermiso>()
                    };
                    rp.Permiso.NombrePermiso ??= string.Empty;
                    rp.Permiso.RolPermisos ??= new List<RolPermiso>();
                }

                return rol;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener rol por Id: {ex.Message}");

                // En caso de error, devolvemos un objeto seguro
                return new Rol
                {
                    IdRol = 0,
                    NombreRol = string.Empty,
                    RolPermisos = new List<RolPermiso>()
                };
            }
        }


        //agregar
        public async Task<ResultadoOperacionR> AgregarRolAsync(Rol rol)
        {
            try
            {
                //AnyAsync Qué hace: Devuelve un booleano (true o false) indicando si existe al menos un elemento que cumpla la condición.
                //Uso típico: Cuando solo quieres saber si existe un registro, sin traerlo completo.
                var rolExiste = await _context.Roles.AnyAsync(r => r.NombreRol == rol.NombreRol);
                if (rolExiste)
                    return new ResultadoOperacionR { Exito = false, Error = "El rol ya existe." };

                _context.Roles.Add(rol);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionR { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionR { Exito = false, Error = ex.Message };
            }
        }

        //Actualziar
        public async Task<ResultadoOperacionR> ActualizarRolAsync(Rol rol)
        {
            //FindAsync Qué hace: Busca un registro por su clave primaria y devuelve el objeto completo si lo encuentra, o null si no existe.
            //Uso típico: Recuperar directamente una entidad por su clave primaria.
            try
            {
                //validar que el rol existe
                var rolDb = await _context.Roles.FindAsync(rol.IdRol);
                if (rolDb == null)
                    return new ResultadoOperacionR { Exito = false, Error = "Rol no encontrado." };

                //validar que el nuevo nombre no este duplicado
                var existeNombre = await _context.Roles.AnyAsync(r => r.NombreRol == rol.NombreRol && r.IdRol != rol.IdRol);
                if (existeNombre)
                    return new ResultadoOperacionR { Exito = false, Error = "El rol ya existe." };

                rolDb.NombreRol = rol.NombreRol;
                await _context.SaveChangesAsync();

                return new ResultadoOperacionR { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionR { Exito = false, Error = $"Error inesperado: {ex.Message}" };
            }
        }

        //elimianr
        public async Task<ResultadoOperacionR> EliminarRolAsync(int id)
        {
            try
            {
                var rolDb = await _context.Roles.FindAsync(id);
                if (rolDb == null)
                    return new ResultadoOperacionR { Exito = false, Error = "Rol no encontrado." };
                //nuevo
                //var tieneUsuarios = await _context.Usuarios.AnyAsync(u => u.IdRol == id);
                //if (tieneUsuarios)
                //    return new ResultadoOperacionR { Exito = false, Error = "No se puede eliminar un rol con usuarios asignados." };
                //validar permisos
                var tienePermisos = await _context.RolPermiso.AnyAsync(rp => rp.IdRol == id);
                if (tienePermisos)
                    return new ResultadoOperacionR { Exito = false, Error = "No se puede eliminar un Rol asignado a permisos." };

                _context.Roles.Remove(rolDb);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionR { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionR { Exito = false, Error = ex.Message };
            }
        }
    }
}