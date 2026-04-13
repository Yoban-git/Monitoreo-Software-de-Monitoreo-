using Marinela.Models.DatosSensores;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace Marinela.ViewModels.PDFController
{
    //QuestPDF.Settings.License = LicenseType.
    public class PdfService
    {
        public byte[] GenerarPdf(List<DatoSensor> datos, string titulo, DateTime desde, DateTime hasta)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // Cabecera
                    page.Header()
                        .Column(col =>
                        {
                            col.Item().Text(titulo).FontSize(20).Bold();
                            col.Item().Text($"Rango: {desde:dd/MM/yyyy} - {hasta:dd/MM/yyyy}").FontSize(12);
                        });

                    // Contenido
                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(40);   // #
                                columns.RelativeColumn();     // Sensor
                                columns.ConstantColumn(80);   // Dato
                                columns.RelativeColumn();     // Fecha
                            });

                            // Encabezados
                            table.Header(header =>
                            {
                                header.Cell().Text("#").Bold();
                                header.Cell().Text("Sensor").Bold();
                                header.Cell().Text("Dato").Bold();
                                header.Cell().Text("Creado En").Bold();
                            });

                            // Filas dinámicas
                            int index = 1;
                            foreach (var item in datos)
                            {
                                table.Cell().Text(index.ToString());
                                table.Cell().Text(item.Sensor.Nombre);
                                table.Cell().Text(item.Dato.ToString("F2"));
                                table.Cell().Text(item.Creado_En.ToString("dd/MM/yyyy HH:mm:ss"));
                                index++;
                            }
                        });

                   
                });
            });

            return document.GeneratePdf();
        }
    }
}
