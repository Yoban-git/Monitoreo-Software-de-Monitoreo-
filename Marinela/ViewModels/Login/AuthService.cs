using Marinela.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.Login
{
    // Servicio de autenticación responsable de gestionar el estado del usuario,
    // realizar el proceso de login/logout y proporcionar utilidades relacionadas
    // con permisos dentro de la aplicación.
    public class AuthService
    {
        private readonly AppDbContext _context;
        // Evento que se dispara cuando cambia el estado de autenticación.
        public event Action? OnChange;
        // Indica si hay un usuario actualmente autenticado.
        public bool IsLoggedIn { get; private set; }
        // Nombre del usuario conectado.
        public string? CurrentUser { get; private set; }
        // Identificador único del usuario autenticado.
        public Guid CurrentUserId { get; private set; }
        // Correo electrónico del usuario actual.
        public string? CurrentUserEmail { get; private set; }
        // Id del rol asignado al usuario actual.
        public int? CurrentUserRol { get; private set; }
        // Nombre del rol del usuario actual.
        public string CurrenRolName {get; private set; }
        // Lista de permisos asociados al usuario autenticado.
        public List<string> CurrentUserPermisos { get; private set; } = new();
        // Construye una nueva instancia AuthService
        // context --> Contexto de la base de datos para consultas
        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        // Intenta autenticar al usuario con las credenciales proporcionadas.
        // username --> Nombre de usuario o correo electrónico.</param>
        // password --> Contraseña en texto claro.</param>
        // regresa un Objeto ResultadoLogin con éxito y mensaje
        public async Task<ResultadoLogin> Login(string username, string password)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return new ResultadoLogin
                    {
                        Exito = false,
                        Mensaje = "Usuario y contraseña son requeridos"
                    };
                }

                // Buscar usuario por username o email (SIN filtrar por Activo)
                var user = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserName == username || u.Email == username);
                if (user == null)
                {
                    return new ResultadoLogin
                    {
                        Exito = false,
                        Mensaje = "Usuario o contraseña incorrectos"
                    };
                }

                // Verificar si el usuario está desactivado ANTES de verificar la contraseña
                if (!user.Activo)
                {
                    return new ResultadoLogin
                    {
                        Exito = false,
                        Mensaje = "Tu cuenta ha sido desactivada. Contacta al administrador."
                    };
                }

                // Verificar contraseña usando BCrypt
                bool passwordValida = BCrypt.Net.BCrypt.Verify(password, user.Password);
                //si la contraseña no es válida, se devuelve un mensaje genérico para no revelar si el usuario existe o no
                if (!passwordValida)
                {
                    return new ResultadoLogin
                    {
                        Exito = false,
                        Mensaje = "Usuario o contraseña incorrectos"
                    };
                }
                // Obtener el nombre del rol del usuario para mostrarlo en la interfaz
                var rol = await _context.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.IdRol == user.IdRol);
                string? nombreRol = rol?.NombreRol;

                // Login exitoso
                IsLoggedIn = true;
                //Guarda el nombre del usuario, su id, email, rol y permisos en el estado para que puedan ser accedidos desde cualquier parte de la aplicación
                CurrentUser = user.Nombre;
                CurrentUserId = user.Id;
                CurrentUserEmail = user.Email;
                CurrentUserRol = user.IdRol;
                // Cargar permisos desde la BD 
                CurrentUserPermisos = await _context.RolPermiso.Where(rp => rp.IdRol == user.IdRol).Select(rp => rp.Permiso.NombrePermiso).ToListAsync();//nuevo
                CurrenRolName = nombreRol;
                NotifyStateChanged();

                return new ResultadoLogin
                {
                    Exito = true,
                    Mensaje = "Inicio de sesión exitoso"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Login: {ex.Message}");
                return new ResultadoLogin
                {
                    Exito = false,
                    Mensaje = "Error al iniciar sesión. Intenta nuevamente."
                };
            }
        }
        // Verifica si el usuario actual posee un permiso específico.
        // permiso --> Nombre del permiso a evaluar.
        //True si el permiso existe en la lista del usuario.</returns>
        public bool TienePermiso(string permiso) => CurrentUserPermisos.Contains(permiso);
        // Cierra la sesión del usuario y borra la información almacenada en el estado.
        public void Logout()
        {
            IsLoggedIn = false;
            CurrentUser = null;
            CurrentUserId = Guid.Empty;
            CurrentUserEmail = null;
            CurrentUserRol = null;
            CurrentUserPermisos.Clear();//nuevo
            CurrenRolName = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
    // Representa el resultado de una operación de inicio de sesión.
    public class ResultadoLogin
    {
        // Indica si la operación fue exitosa.
        public bool Exito { get; set; }
        // Mensaje explicativo o de error asociado al resultado.
        public string Mensaje { get; set; } = string.Empty;
    }
}
