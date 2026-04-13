using BCrypt.Net;
using Marinela.Models.Data;
using Marinela.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.UsuarioController
{
    public class ResultadoOperacionU
    {
        public bool Exito { get; set; }
        public string Error { get; set; } = string.Empty;
    }
    public class UsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        //listar todos los datos
        public async Task<List<Usuario>> ObtenerTodosUsuariosAsync()
        {
            try
            {
                return await _context.Usuarios.Include(u => u.Rol)
                                                .OrderByDescending(u => u.Creado_En)
                                                .ToListAsync() ?? new List<Usuario>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        //listar solo los datos activos
        public async Task<List<Usuario>> ObtenerUsuariosActivosAsync()
        {
            try
            {
                return await _context.Usuarios.Include(u => u.Rol)
                                                .OrderByDescending(u => u.Creado_En)
                                                .Where(u => u.Activo)
                                                .ToListAsync() ?? new List<Usuario>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuarios activos: {ex.Message}");
                return new List<Usuario>();
            }
        }

        //obtener un usuario por su Gui para actualziar
        public async Task<Usuario?> ObtenerUsuarioPorGuid(Guid idUsuario)
        {
            try
            {
                return await _context.Usuarios.Include(u => u.Rol)
                                                .FirstOrDefaultAsync(u => u.Id == idUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"El usuario no existe o se registro  con otro nombre {ex.Message}");
                return null;
            }
        }

        //crear
        public async Task<ResultadoOperacionU> CrearUsuarioAsync(Usuario usuario)
        {
            try
            {
                // Validar que el rol exista
                var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == usuario.IdRol);
                if (!rolExiste)
                    return new ResultadoOperacionU { Exito = false, Error = "El rol especificado no existe." };

                // Asignar nuevo Guid
                usuario.Id = Guid.NewGuid();

                // Asignar fechas de creación y modificación
                usuario.Creado_En = DateTime.Now;
                usuario.Modificado_En = DateTime.Now;
                usuario.Activo = true;

                // Validar y hashear contraseña
                if (string.IsNullOrWhiteSpace(usuario.Password))
                {
                    return new ResultadoOperacionU { Exito = false, Error = "La contraseña es obligatoria." };
                }
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

                // Agregar y guardar
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionU { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionU { Exito = false, Error = $"Error al crear usuario: {ex.Message}" };
            }
        }


        //Actualziar un Usuario
        public async Task<ResultadoOperacionU> ActualizarUsuarioAsync(Usuario usuario)
        {
            try
            {
                // Buscar el usuario en la base de datos
                var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuario.Id);
                if (usuarioExistente == null)
                {
                    return new ResultadoOperacionU { Exito = false, Error = "El usuario no existe." };
                }

                // Validar que el rol exista
                var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == usuario.IdRol);
                if (!rolExiste)
                {
                    return new ResultadoOperacionU { Exito = false, Error = "El rol especificado no existe." };
                }

                // Actualizar propiedades
                usuarioExistente.Nombre = usuario.Nombre ?? string.Empty;
                usuarioExistente.UserName = usuario.UserName ?? string.Empty;
                usuarioExistente.Email = usuario.Email ?? string.Empty;
                usuarioExistente.IdRol = usuario.IdRol;

                // Actualizar fecha de modificación
                usuarioExistente.Modificado_En = DateTime.Now;

                // Guardar cambios
                _context.Usuarios.Update(usuarioExistente);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionU { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionU { Exito = false, Error = $"Error al actualizar usuario: {ex.Message}" };
            }
        }


        //elimianr un usaurio
        public async Task<ResultadoOperacionU> EliminarUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var usuarioDb = await _context.Usuarios.FindAsync(idUsuario);
                if (usuarioDb == null)
                    return new ResultadoOperacionU { Exito = false, Error = "Usuario no encontrado." };

                var tieneRol = await _context.Roles.AnyAsync(r => r.IdRol == usuarioDb.IdRol);
                if (tieneRol)
                    return new ResultadoOperacionU { Exito = false, Error = "No se puede eliminar el usuario porque tiene un rol asignado." };
                
                _context.Usuarios.Remove(usuarioDb);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionU { Exito = true };
            }
            catch (Exception ex)
            {

                return new ResultadoOperacionU { Exito = false, Error = ex.Message };
            }
        }

        //Activar/Desactivar cuentas
        public async Task<ResultadoOperacionU> CambiarEstadoUsuarioAsync(Guid idUsuario)
        {
            try
            {
                var usuarioDb = await _context.Usuarios.FindAsync(idUsuario);
                if (usuarioDb == null)
                    return new ResultadoOperacionU { Exito = false, Error = "Usuario no encontrado." };

                usuarioDb.Activo = !usuarioDb.Activo; //niega el valor que trae por defecto
                _context.Usuarios.Update(usuarioDb);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionU { Exito = true };
            }
            catch (Exception ex)
            {

                return new ResultadoOperacionU { Exito = false, Error = ex.Message };
            }
        }


        // Cambia la contraseña de un usuario existente en la base de datos.
        // usuarioId -->GUID del usuario al que se le cambiará la contraseña
        // nuevaContraseña --> Nueva contraseña en texto plano que será encriptada antes de guardarse
        // Un objeto ResultadoOperacionU indicando si la operación fue exitosa o si ocurrió algún error.
        public async Task<ResultadoOperacionU> CambiarContraseniaAsync(Guid usuarioId, string nuevaContraseña)
        {
            try
            {
                // Buscar el usuario en la base de datos por su GUID
                var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);
                if (usuarioExistente == null)
                {
                    // Si no existe, devolver error
                    return new ResultadoOperacionU { Exito = false, Error = "El usuario no existe." };
                }

                // Validar que la nueva contraseña no esté vacía o compuesta solo por espacios
                if (string.IsNullOrWhiteSpace(nuevaContraseña))
                {
                    return new ResultadoOperacionU { Exito = false, Error = "La nueva contraseña no puede estar vacía." };
                }
                // Verificar que la nueva contraseña no sea la misma que la actual
                if (BCrypt.Net.BCrypt.Verify(nuevaContraseña, usuarioExistente.Password))
                {
                    return new ResultadoOperacionU { Exito = false, Error = "La nueva contraseña no puede ser igual a la anterior." };
                }

                // Encriptar la nueva contraseña usando BCrypt antes de guardarla
                usuarioExistente.Password = BCrypt.Net.BCrypt.HashPassword(nuevaContraseña);

                // Actualizar la fecha de modificación
                usuarioExistente.Modificado_En = DateTime.Now;

                // Marcar el usuario como actualizado en el contexto
                _context.Usuarios.Update(usuarioExistente);

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Devolver resultado exitoso
                return new ResultadoOperacionU { Exito = true };
            }
            catch (Exception ex)
            {
                // Capturar cualquier excepción y devolver un error con el mensaje
                return new ResultadoOperacionU { Exito = false, Error = $"Error al cambiar contraseña: {ex.Message}" };
            }
        }
    }
}