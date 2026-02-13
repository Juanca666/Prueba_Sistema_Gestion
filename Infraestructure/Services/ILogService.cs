using System.IO;
namespace SistemaGestionPeliculas.Infraestructure.Services;

public interface ILogService
{
	//Parametros que registran la accion realizada.
	void LogPeliculas(string accion, int peliculaId, decimal precio, int categoriaId);
}

// Implementacion servicio de logs.
public class LogService : ILogService
{
    // registrar la accion realizada en un archivo de texto.
    private readonly string _logFilePath;
    // Previene que varios hilos escriban al mismo tiempo en el archivo de log, lo que podría causar problemas de concurrencia.
    private readonly object _lock = new object();



    public LogService (IConfiguration configuration)
    {
        // Obtener la ruta del directorio de logs desde la configuración, o usar "Logs" por defecto.
        var logDirectory = configuration["LogSettings:Directory"] ?? "Logs";

        // Verificar si el directorio de logs existe, y si no, crearlo.
        if (!Directory.Exists(logDirectory))
        
            Directory.CreateDirectory(logDirectory);

        // Establecer la ruta completa del archivo de log, combinando el directorio con el nombre del archivo.
        _logFilePath = Path.Combine(logDirectory, "peliculas.log");
    }

       

    public void LogPeliculas(string accion, int peliculaId, decimal precio, int categoriaId)
    {
        // Crear el mensaje de log con la información proporcionada y la fecha/hora actual. Fixed-point precio con 2 decimales
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {accion} | {peliculaId} | {precio:F2} | {categoriaId}";

         // Bloquear el acceso al archivo de log para evitar que varios hilos escriban al mismo tiempo.
         lock (_lock)
            {
            // Escribir el mensaje de log en el archivo, agregándolo al final del mismo.
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }

    }
}