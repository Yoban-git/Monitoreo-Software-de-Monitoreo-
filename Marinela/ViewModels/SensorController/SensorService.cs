using Marinela.Models.Data;
using Marinela.Models.Sensores;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace Marinela.ViewModels.SensorController
{
    public class ResultadoOperacionSr
    {
        public bool Exito { get; set; }
        public string Error { get; set; } = string.Empty;
    }
    public class SensorService
    {
        // Contexto de la base de datos
        private readonly AppDbContext _context;
        // Constructor que recibe el contexto de la base de datos
        public SensorService(AppDbContext context)
        {
            _context = context;
        }
        //listar todos los sensores
        public async Task<List<Sensor>> ListarSensoresAsync()
        {
            try
            {
                // Devuelve todos los sensores
                return await _context.Sensores
                                    .Include(s => s.Datos) // Incluye los datos relacionados de cada sensor
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar sensores: {ex.Message}");
                return new List<Sensor>();
            }
        }

        //listar solo los sensores Activos
        public async Task<List<Sensor>> ListarSensoresActivosAsync()
        {
            try
            {
                return await _context.Sensores
                                    .Include(s => s.Datos) // Incluye los datos relacionados de cada sensor
                                    .Where(s => s.Activo)  // Filtra solo los sensores activos
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar sensores activos: {ex.Message}");
                return new List<Sensor>();
            }
        }

        //listar sensores por tag
        public async Task<Sensor?> ObtenerSensorPorTagAsync(string tag)
        {
            try
            {
                return await _context.Sensores
                                        .Include(s => s.Datos) // Incluye los datos relacionados de cada sensor
                                                               // FirstOrDefaultAsync devuelve el primer elemento que cumple la condición o null si no se encuentra
                                        .FirstOrDefaultAsync(s => s.Tag == tag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener sensor por Tag: {ex.Message}");
                return null;
            }
        }

        //Obtener Datos de Sensor por Id para actualizar
        public async Task<Sensor?> ObtenerSensorPorIdAsync(int idSensor)
        {
            try
            {
                // FirstOrDefaultAsync devuelve el primer elemento que cumple la condición o null si no se encuentra
                var sensor = await _context.Sensores.FirstOrDefaultAsync(s => s.IdSensor == idSensor);
                return sensor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el sensor: {ex.Message}");
                return null;
            }
        }

        //Guardar un sensor
        public async Task<ResultadoOperacionSr> GuardarSensorAsync(Sensor sensor)
        {
            try
            {
                //validacion para valores minimos
                if (sensor.ValorMinimo <= 0 || sensor.ValorMaximo <= 0)
                    return new ResultadoOperacionSr { Exito = false, Error = "tus valores deben ser mayores a 1" };
                //validacion para duplicidad
                var sensorExiste = await _context.Sensores.AnyAsync(s => s.Nombre == sensor.Nombre || s.Tag == sensor.Tag);
                if (sensorExiste)
                    return new ResultadoOperacionSr { Exito = false, Error = "El sensor ya existe" };

                _context.Sensores.Add(sensor);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionSr { Exito = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResultadoOperacionSr { Exito = false, Error = ex.Message };
            }
        }

        //Actualziar un sensor
        public async Task<ResultadoOperacionSr> ActualizarSensorAsync(Sensor sensor)
        {
            try
            {
                //verificar qeu existe el sensor
                var sensorDb = await _context.Sensores.FindAsync(sensor.IdSensor);

                if (sensorDb == null)
                    return new ResultadoOperacionSr { Exito = false, Error = "Sensor no encontrado." };

                //Actualziar solo campos necesarios
                sensorDb.Nombre = sensor.Nombre;
                sensorDb.Tag = sensor.Tag;
                sensorDb.ValorMinimo = sensor.ValorMinimo;
                sensorDb.ValorMaximo = sensor.ValorMaximo;
                sensorDb.Modificado_En = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ResultadoOperacionSr { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSr { Exito = false, Error = $"Error inesperado: {ex.Message}" };
            }
        }
        //Actualziar rango de un sensor
        // Actualizar solo valores de mínimo y máximo
        public async Task<ResultadoOperacionSr> ActualizarRangoAsync(int idSensor, double minimo, double maximo)
        {
            try
            {
                var sensor = await _context.Sensores.FindAsync(idSensor);

                if (sensor == null)
                {
                    return new ResultadoOperacionSr
                    {
                        Exito = false,
                        Error = "Sensor no encontrado."
                    };
                }

                // Validación: valores mayores a 0
                if (minimo <= 0 || maximo <= 0)
                {
                    return new ResultadoOperacionSr
                    {
                        Exito = false,
                        Error = "Los valores mínimo y máximo deben ser mayores a 0."
                    };
                }

                // Validación: rango lógico (mínimo < máximo)
                if (minimo >= maximo)
                {
                    return new ResultadoOperacionSr
                    {
                        Exito = false,
                        Error = "El valor mínimo debe ser menor que el valor máximo."
                    };
                }

                // Actualizar valores
                sensor.ValorMinimo = minimo;
                sensor.ValorMaximo = maximo;
                sensor.Modificado_En = DateTime.UtcNow; // actualizar fecha de modificación

                await _context.SaveChangesAsync();

                return new ResultadoOperacionSr { Exito = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ResultadoOperacionSr { Exito = false, Error = ex.Message };
            }
        }



        //Eliminar Sensor
        public async Task<ResultadoOperacionSr> EliminarSensorAsync(int idSensor)
        {
            try
            {
                var sensorDb = await _context.Sensores.FindAsync(idSensor);
                if (sensorDb == null)
                    return new ResultadoOperacionSr { Exito = false, Error = "Sensor no encontrado." };

                _context.Sensores.Remove(sensorDb);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionSr { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSr { Exito = false, Error = ex.Message };
            }
        }

        //Activar/Desactivar sensor
        public async Task<ResultadoOperacionSr> CambiarEstadoSensorAsync(int idSensor)
        {
            try
            {
                var sensor = await _context.Sensores.FindAsync(idSensor);
                if (sensor == null)
                    return new ResultadoOperacionSr { Exito = false, Error = "Sensor no encontrado." };

                sensor.Activo = !sensor.Activo;
                _context.Sensores.Update(sensor);
                await _context.SaveChangesAsync();
                return new ResultadoOperacionSr { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSr { Exito = false, Error = $"Error inesperado: {ex.Message}" };
            }
        }
    }
}
