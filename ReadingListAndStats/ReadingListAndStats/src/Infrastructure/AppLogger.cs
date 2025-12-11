using Serilog;

namespace ReadingList.src.Infrastructure
{
    public static class AppLogger
    {
        public static ILogger Logger { get; private set; } = null!;

        public static void Initialize()
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/readinglist-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Logger = Logger;

            Logger.Information("Logger initialized");
        }

        public static void Close() => Log.CloseAndFlush();
    }
}
