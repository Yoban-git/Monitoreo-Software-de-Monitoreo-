using System.ComponentModel.DataAnnotations;

namespace Marinela.Models.DatosExcel
{
    public class DatoExcel
    {
        [Key]
        public int IdDatoE { get; set; }
        public string? TagSensor { get; set; } // varchar
        public double Dato { get; set; } // float
        public DateTime Creado_En { get; set; } // datetime
    }
}
