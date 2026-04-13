using Marinela.Models.DatosSensores;
using System.ComponentModel.DataAnnotations;

namespace Marinela.Models.Sensores
{
    public class Sensor
    {
        [Key]
        public int IdSensor { get; set; }

        [Required(ErrorMessage = "El nombre del sensor es obligatorio")]
        public string Nombre { get; set; } = string.Empty; // inicializado

        public string Tag { get; set; } = string.Empty; // inicializado
        public double ValorMinimo { get; set; } = 1;
        public double ValorMaximo { get; set; } = 1;
        public DateTime Creado_En { get; set; } = DateTime.Now;
        public DateTime Modificado_En { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        // Relación con Datos_Sensores
        public ICollection<DatoSensor> Datos { get; set; } = new List<DatoSensor>();
    }
}
