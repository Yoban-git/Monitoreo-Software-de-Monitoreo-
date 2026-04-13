using Marinela.Models.Data;
using Marinela.Models.DatosSensores;
using Microsoft.EntityFrameworkCore;

namespace Marinela.ViewModels.DatoSensorController
{
    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; } = [];
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
    public class ResultadoOperacionSD
    {
        public bool Exito { get; set; }
        public string? Error { get; set; }
    }
    public class DatoSensorService
    {
        //
        private readonly AppDbContext _context;
        //Constructor que recibe el contexto de la base de datos para realizar consultas
        public DatoSensorService(AppDbContext context)
        {
            _context = context;
        }

        // Listar Datos Activos
        public async Task<List<DatoSensor>> ListarDatosActivosAsync()
        {
            return await _context.DatosSensores
                                                .Where(d => d.Sensor.Activo)
                                                .OrderByDescending(d => d.Creado_En)
                                                .ToListAsync();
        }
        // Listar datos por IdSensor ListarDatosPorSensorAsync
        public async Task<PaginatedResult<DatoSensor>> ListarDatosPorSensorAsync(int idSensor, DateTime desde, DateTime hasta,
            int pagina = 1, int tamanoPagina = 10)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");

            var query = _context.DatosSensores
                .Where(d => d.IdSensor == idSensor && d.Creado_En >= desde && d.Creado_En <= hasta)
                .OrderBy(d => d.Creado_En);

            var total = await query.CountAsync();

            var datos = await query
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PaginatedResult<DatoSensor>
            {
                Data = datos,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(total / (double)tamanoPagina),
                TotalRegistros = total
            };
        }

        // Obtener un dato por su Id para actualizar
        public async Task<DatoSensor?> ObtenerDatoPorIdAsync(int idDato)
        {
            try
            {
                if (idDato <= 0)
                    return null;

                return await _context.DatosSensores
                                    .Include(d => d.Sensor) // incluir relación con Sensor
                                    .FirstOrDefaultAsync(d => d.IdDato == idDato);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener dato por Id: {ex.Message}");
                return null;
            }
        }
        // Guardar un nuevo dato de sensor
        public async Task<ResultadoOperacionSD> GuardarDatoAsync(DatoSensor dato)
        {
            try
            {
                // Validación: objeto nulo
                if (dato == null)
                {
                    return new ResultadoOperacionSD
                    {
                        Exito = false,
                        Error = "El dato no puede ser nulo."
                    };
                }
                // Validación: IdSensor válido
                if (dato.IdSensor <= 0)
                {
                    return new ResultadoOperacionSD
                    {
                        Exito = false,
                        Error = "Sensor invalido."
                    };
                }

                // Validación: existencia del sensor
                var sensorExiste = await _context.Sensores.AnyAsync(s => s.IdSensor == dato.IdSensor);
                if (!sensorExiste)
                {
                    return new ResultadoOperacionSD
                    {
                        Exito = false,
                        Error = "El sensor no existe."
                    };
                }
                // Validación: dato mayor a 0 (si aplica tu regla de negocio)
                if (dato.Dato <= 0)
                {
                    return new ResultadoOperacionSD
                    {
                        Exito = false,
                        Error = "El valor del dato debe ser mayor a 0."
                    };
                }


                // Fecha de creación automática
                dato.Creado_En = DateTime.UtcNow;

                // Guardar
                _context.DatosSensores.Add(dato);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionSD { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSD { Exito = false, Error = ex.Message };
            }
        }

        //ingresar una coleccion de datos
        // Guardar múltiples datos de sensores
        public async Task<ResultadoOperacionSD> GuardarColeccionDatosAsync(IEnumerable<DatoSensor> datos)
        {
            try
            {
                if (datos == null || !datos.Any())
                {
                    return new ResultadoOperacionSD
                    {
                        Exito = false,
                        Error = "La lista de datos no puede estar vacía."
                    };
                }

                foreach (var dato in datos)
                {
                    // Validación: objeto nulo
                    if (dato == null)
                    {
                        return new ResultadoOperacionSD
                        {
                            Exito = false,
                            Error = "Uno de los datos es nulo."
                        };
                    }

                    // Validación: IdSensor válido
                    if (dato.IdSensor <= 0)
                    {
                        return new ResultadoOperacionSD
                        {
                            Exito = false,
                            Error = $"Sensor inválido en dato con valor {dato.Dato}."
                        };
                    }

                    // Validación: existencia del sensor
                    var sensorExiste = await _context.Sensores.AnyAsync(s => s.IdSensor == dato.IdSensor);
                    if (!sensorExiste)
                    {
                        return new ResultadoOperacionSD
                        {
                            Exito = false,
                            Error = $"El sensor con Id {dato.IdSensor} no existe."
                        };
                    }

                    // Validación: dato mayor a 0
                    if (dato.Dato <= 0)
                    {
                        return new ResultadoOperacionSD
                        {
                            Exito = false,
                            Error = $"El valor del dato debe ser mayor a 0 (Sensor {dato.IdSensor})."
                        };
                    }

                    // Fecha de creación automática
                    dato.Creado_En = DateTime.UtcNow;

                    // Agregar al contexto
                    _context.DatosSensores.Add(dato);
                }

                // Guardar todos los cambios en una sola operación
                await _context.SaveChangesAsync();

                return new ResultadoOperacionSD { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSD { Exito = false, Error = ex.Message };
            }
        }


        //Actualziar un dato de sensor
        public async Task<ResultadoOperacionSD> ActualizarDatoAsync(DatoSensor dato)
        {
            try
            {
                var datoDb = await _context.DatosSensores.FindAsync(dato.IdDato);

                if (datoDb == null)
                {
                    return new ResultadoOperacionSD
                    {
                        Exito = false,
                        Error = "Dato no encontrado"
                    };
                }

                // Actualizar solo los campos necesarios
                datoDb.Dato = dato.Dato;
                datoDb.IdSensor = dato.IdSensor;

                await _context.SaveChangesAsync();
                return new ResultadoOperacionSD { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSD { Exito = false, Error = ex.Message };
            }
        }

        //Eliminar un dato de sensor
        public async Task<ResultadoOperacionSD> EliminarDatoAsync(int IdDato)
        {
            try
            {
                var datoDb = await _context.DatosSensores.FindAsync(IdDato);

                if (datoDb == null)
                    return new ResultadoOperacionSD { Exito = false, Error = "Dato no encontrado" };

                _context.DatosSensores.Remove(datoDb);
                await _context.SaveChangesAsync();

                return new ResultadoOperacionSD { Exito = true };
            }
            catch (Exception ex)
            {
                return new ResultadoOperacionSD { Exito = false, Error = ex.Message };
            }
        }

        //Datos para graficar
        // Obtener todos los datos de un sensor específico
        public async Task<List<DatoSensor>> GetDatosPorSensorAsync(int idSensor)
        {
            return await _context.DatosSensores
                .Where(d => d.IdSensor == idSensor)
                .OrderBy(d => d.Creado_En)
                .ToListAsync();
        }
        // Obtener datos filtrados por rango de fechas
        public async Task<List<DatoSensor>> GetDatosPorFechaAsync(int idSensor, DateTime desde, DateTime hasta)
        {
            if (desde > hasta)
                throw new ArgumentException("La fecha de inicio no puede ser mayor que la fecha de fin.");
            return await _context.DatosSensores
                .Where(d => d.IdSensor == idSensor && d.Creado_En >= desde && d.Creado_En <= hasta)
                .OrderBy(d => d.Creado_En)
                .ToListAsync();
        }
        //lo nuevo
        // Obtener datos desde la tabla DatosExcel por rango de fechas
        public async Task<List<DatoSensor>> GetDatosExcelPorFechaAsync(int idSensor, DateTime desde, DateTime hasta)
        {
            // Obtener el Tag del sensor
            var sensor = await _context.Sensores.FirstOrDefaultAsync(s => s.IdSensor == idSensor);
            if (sensor == null) return new List<DatoSensor>();

            // Mapear DatosExcel -> SensorData
            return await _context.DatosExcel
                .Where(d => d.TagSensor == sensor.Tag && d.Creado_En >= desde && d.Creado_En <= hasta)
                .Select(d => new DatoSensor
                {
                    IdSensor = idSensor,
                    Dato = d.Dato, //Dato = Math.Round(d.Dato, 2)
                    Creado_En = d.Creado_En
                })
                .OrderBy(d => d.Creado_En)
                .ToListAsync();
        }
    }
}
