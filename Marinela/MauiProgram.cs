using ApexCharts;
using Marinela.Models.Data;
using Marinela.ViewModels.DatoSensorController;
using Marinela.ViewModels.DatosExcelController;
using Marinela.ViewModels.Login;
using Marinela.ViewModels.PDFController;
using Marinela.ViewModels.PermisoController;
using Marinela.ViewModels.RolController;
using Marinela.ViewModels.RolPermisoController;
using Marinela.ViewModels.SensorController;
using Marinela.ViewModels.UsuarioController;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;

namespace Marinela
{
    public static class MauiProgram
    {
        
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();


            //definir rurta de archivo de configuracion externo
            var externalPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Marinela",
                "appsettings.json");

            //si no existe el archivo, crear archivo de configuracion externo al inciar el programa
            if (!File.Exists(externalPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(externalPath)!);

                var defaultConfig = @"{
                  ""DatabaseSettings"": {
                    ""Motor"": ""SQL_SERVER"",
                    ""Server"": ""DESKTOP-7HKQF3L"",
                    ""Database"": ""Monitoreo"",
                    ""User"": ""sa"",
                    ""Password"": ""Adm1nTr4baj0"",
                    ""Port"":""""
                  }
                }";
                File.WriteAllText(externalPath, defaultConfig);
            }
            // Cargar configuración desde appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            builder.Configuration.AddJsonFile(externalPath, optional: true, reloadOnChange: true);

            //motor de base de datos
            var motor = builder.Configuration["DatabaseSettings:Motor"];

            var server = builder.Configuration["DatabaseSettings:Server"];
            var database = builder.Configuration["DatabaseSettings:Database"];
            var user = builder.Configuration["DatabaseSettings:User"];
            var password = builder.Configuration["DatabaseSettings:Password"];
            var port = builder.Configuration["DatabaseSettings:Port"];

            //usa la cadena de conexion del JSON
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                if (motor == "SQL_SERVER")
                {
                    options.UseSqlServer($"Server={server};Database={database};User Id={user};Password={password};Port={port};TrustServerCertificate=true;");
                }
                else if (motor == "MySQL")
                {
                    options.UseMySQL($"Server={server};Database={database};User={user};Password={password};Port={port}");
                }
                else
                    throw new Exception("Proveedor de base de datos no soportado. Use SqlServer, MySql o PostgreSql.");

            });


            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<PdfService>();

            builder.Services.AddScoped<UsuarioService>();
            builder.Services.AddScoped<RolService>();
            builder.Services.AddScoped<PermisoService>();
            builder.Services.AddScoped<RolPermisoService>();
            builder.Services.AddScoped<SensorService>();
            builder.Services.AddScoped<DatoSensorService>();
            builder.Services.AddScoped<DatosExcelService>();
            builder.Services.AddApexCharts();
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 3500;
                config.SnackbarConfiguration.HideTransitionDuration = 300;
                config.SnackbarConfiguration.ShowTransitionDuration = 300;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
