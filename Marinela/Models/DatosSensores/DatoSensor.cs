using Marinela.Models.Sensores;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marinela.Models.DatosSensores
{
    public class DatoSensor
    {
        [Key]
        public int IdDato { get; set; }

        [ForeignKey("Sensor")]
        [Range(1, int.MaxValue, ErrorMessage = "El campo IdSensor es obligatorio.")]
        public int IdSensor { get; set; }

        // Valor del dato, inicializado en 0 para evitar nulos
        public double Dato { get; set; } = 0;

        // Fecha de creación, inicializada automáticamente
        public DateTime Creado_En { get; set; } = DateTime.Now;

        // Relación con Sensor, nunca será null
        public Sensor Sensor { get; set; } = null!;
    }
}

