using System;
using Serilog;
using Serilog.Events;

namespace ViberLounge.Infrastructure.Logging
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;

        public LoggerService()
        {
            string apiProjectPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            
            if (Directory.Exists(Path.Combine(apiProjectPath, "bin")))
            {
                apiProjectPath = Directory.GetCurrentDirectory();
            }
            
            string logDirectory = Path.Combine(apiProjectPath, "logs");
            string logFilePath = Path.Combine(logDirectory, "viber-lounge-.txt");
            
            try
            {
                // Verifica se o diretório de logs existe, se não, cria
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                    Console.WriteLine($"Diretório de logs criado em: {logDirectory}");
                }
                
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .WriteTo.File(logFilePath, 
                        rollingInterval: RollingInterval.Day,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1))
                    .CreateLogger();
                
                _logger.Information("Serviço de logging inicializado com sucesso. Diretório de logs: {LogDirectory}", logDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar o serviço de logging: {ex.Message}");
                _logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            _logger.Error(ex, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
    }
} 