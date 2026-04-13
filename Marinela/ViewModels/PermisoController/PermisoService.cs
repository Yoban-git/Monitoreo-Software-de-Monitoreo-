using Marinela.Models.Data;
using Marinela.Models.Permisos;
using Marinela.Models.RolesPermisos;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.PermisoController
{
    public class ResultadoOperacionP
    {
        public bool Exito { get; set; }
        public string Error { get; set; } = string.Empty;
    }
    public class PermisoService
    {
        private readonly AppDbContext _context;
        // Constructor que recibe el contexto de la base de datos
        public PermisoService(AppDbContext context)
        {
            _context = context;
        }

        //Listar o Read
        public async Task<List<Permiso>> ObtenerTodosPermisosAsync()
        {
            try
            {
                // ToListAsync devuelve una lista de permisos, o una lista vacía si no hay permisos.
                return await _context.Permisos.ToListAsync() ?? new List<Permiso>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener permisos: {ex.Message}");
                return new List<Permiso>();
            }
        }

        // Obtener un permiso por su ID
        public async Task<Permiso?> ObtenerPermisoAsyncPorId(int idPermiso)
        {
            try
            {
                // Busca el permiso por su ID, devuelve null si no se encuentra
                return await _context.Permisos.FirstOrDefaultAsync(p => p.IdPermiso == idPermiso);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"El permiso no existe o se registro  con otro nombre {ex.Message}");
                return null;
            }
        }
        // Crear un nuevo permiso
        public async Task<ResultadoOperacionP> AgregarPermisoAsync(Permiso permiso)
        {
            try
            {
                // Validar que el objeto permiso no sea nulo
                if (permiso == null)
                {
                    return new ResultadoOperacionP { Exito = false, Error = "El permiso proporcionado es nulo." };
                }

                // Normalizar nombre para evitar problemas con nulos o espacios
                var nombre = permiso.NombrePermiso?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return new ResultadoOperacionP { Exito = false, Error = "El nombre del permiso es obligatorio." };
                }

                // Validar si ya existe un permiso con ese nombre
                //AnyAsync devuelve true si existe al menos un permiso con el mismo nombre, o false si no existe ninguno.
                var existe = await _context.Permisos.AnyAsync(p => p.NombrePermiso == nombre);
                if (existe)
                {
                    return new ResultadoOperacionP { Exito = false, Error = "El permiso ya existe en la base de datos." };
                }

                // Asignar valores seguros
                permiso.NombrePermiso = nombre;
                // Asegurar que la lista de relaciones no sea nula para evitar problemas al agregar el permiso
                permiso.RolPermisos = permiso.RolPermisos ?? new List<RolPermiso>();

                // Agregar y guardar
                _context.Permisos.Add(permiso);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionP { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionP { Exito = false, Error = $"Error al agregar permiso: {ex.Message}" };
            }
        }

        //Metodo para actualizar un permiso.
        public async Task<ResultadoOperacionP> ActualizarPermisoAsync(Permiso permiso)
        {
            try
            {
                // Validar que el objeto no sea nulo
                if (permiso == null)
                {
                    return new ResultadoOperacionP
                    {
                        Exito = false,
                        Error = "El permiso proporcionado es nulo."
                    };
                }

                // Buscar el permiso en la base de datos
                var permisoDb = await _context.Permisos.FindAsync(permiso.IdPermiso);

                if (permisoDb == null)
                {
                    return new ResultadoOperacionP
                    {
                        Exito = false,
                        Error = "Permiso no encontrado."
                    };
                }

                // Normalizar nombre para evitar problemas con nulos o espacios
                var nombre = permiso.NombrePermiso?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return new ResultadoOperacionP
                    {
                        Exito = false,
                        Error = "El nombre del permiso es obligatorio."
                    };
                }

                // Verificar que el nuevo nombre no se repita en otro permiso
                var existeNombre = await _context.Permisos
                    .AnyAsync(p => p.NombrePermiso == nombre && p.IdPermiso != permiso.IdPermiso);

                if (existeNombre)
                {
                    return new ResultadoOperacionP
                    {
                        Exito = false,
                        Error = "Ya existe otro permiso con ese nombre."
                    };
                }

                // Actualizar solo los campos necesarios
                permisoDb.NombrePermiso = nombre;

                await _context.SaveChangesAsync();

                return new ResultadoOperacionP { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionP
                {
                    Exito = false,
                    Error = $"Error inesperado: {ex.Message}"
                };
            }
        }


        public async Task<ResultadoOperacionP> EliminarPermisoAsync(int id)
        {
            try
            {
                var permiso = await _context.Permisos.FindAsync(id);
                if (permiso == null)
                    return new ResultadoOperacionP { Exito = false, Error = "Permiso no encontrado." };
                //lo nuevo
                var tieneRoles = await _context.RolPermiso.AnyAsync(rp => rp.IdPermiso == id);
                if (tieneRoles)
                    return new ResultadoOperacionP { Exito = false, Error = "No se puede eliminar un permiso asignado a roles." };

                _context.Permisos.Remove(permiso);
                await _context.SaveChangesAsync();
                return new ResultadoOperacionP { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionP { Exito = false, Error = ex.Message };
            }
        }
    }
}
