using Marinela.Models.Data;
using Marinela.Models.DatosExcel;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.DatosExcelController
{
    public class DatosExcelService
    {
        private readonly AppDbContext _context;

        public DatosExcelService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<DatoExcel>> ObtenerDatosAsync()
        {
            return await _context.DatosExcel.ToListAsync();
        }
        public async Task GuardarDatosAsync(List<DatoExcel> datos)
        {
            _context.DatosExcel.AddRange(datos);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> EliminarDatosExcelAsync()
        {
            try
            {
                var datos = _context.DatosExcel.ToList();

                if (datos.Count == 0)
                {
                    return false; // No hay datos
                }

                _context.DatosExcel.RemoveRange(datos);
                await _context.SaveChangesAsync();
                return true; // Se eliminaron datos
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar datos: {ex.Message}");
                throw;
            }
        }
    }
}