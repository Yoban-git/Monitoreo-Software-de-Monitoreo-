using Marinela.Models.Roles;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marinela.Models.Usuarios
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty; // inicializado

        [Required(ErrorMessage = "El Nombre de Usuario es obligatorio")]
        public string UserName { get; set; } = string.Empty; // inicializado

        [Required(ErrorMessage = "El Correo es obligatorio")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public DateTime Creado_En { get; set; } = DateTime.Now;
        public DateTime Modificado_En { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Relación con Rol
        public int IdRol { get; set; }

        // Se inicializa para evitar null, pero se marca como nullable
        public Rol? Rol { get; set; } = null;
    }
}