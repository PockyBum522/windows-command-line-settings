using Serilog;

namespace WindowsCommandLineSettings;

public static class DependencyBuilders
{
    internal static ILogger BuildLogger(string logPath, ILogger? testingLogger = null)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(ApplicationPaths.LogPath) ?? "");

        if (testingLogger is not null)
        {
            return testingLogger;
        }

        // Otherwise, if it is null, make new logger:
        return new LoggerConfiguration()
            .Enrich.WithProperty("WindowsSetupAssistantApplication", "WindowsSetupAssistantSerilogContext")
            .MinimumLevel.Debug()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();
    }
}