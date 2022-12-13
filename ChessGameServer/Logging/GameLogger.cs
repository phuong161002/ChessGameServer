using System;
using Serilog;

namespace ChessGameServer.Logging
{
    public class GameLogger : IGameLogger
    {
        private readonly ILogger _logger;

        public GameLogger()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("Logging/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void Print(string msg)
        {
            _logger.Information($">>>>>> {msg}");
        }

        public void Info(string info)
        {
            _logger.Information(info);
        }

        public void Warning(string warning, Exception exception = null)
        {
            _logger.Warning(warning, exception);
        }

        public void Error(string error, Exception exception)
        {
            _logger.Error(error, exception);
        }

        public void Error(string error)
        {
            _logger.Error(error);
        }
    }
}