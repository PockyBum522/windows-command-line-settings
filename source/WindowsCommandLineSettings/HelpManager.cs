using Serilog;

namespace WindowsCommandLineSettings;

internal class HelpManager
{
    private readonly ILogger _logger;

    public HelpManager(ILogger logger)
    {
        _logger = logger;
    }

    internal void FetchAppropriateHelpMessage(string[] args)
    {
        throw new NotImplementedException();
    }

    internal void PrintHelpToConsole()
    {
        var message = @$"
            This help is printed when windowsCommandLineSettings.exe is run with no arguments or with -help

            To print this help:
            -help

            To see available settings that can be changed:
            -commands

            To get extended help on a specific setting change:
            -help setting-name

            To explicitly set log file location (Timestamp will be appended to log name automatically):
            -logpath ""{ApplicationPaths.LogAppBasePath}"" 
            Note: The default log path is the above path

            To show and log extra debug messages:            
            -debug debug-level
            Available options for debug-level:
                Warning
                Information
                Error
                Fatal";

        Console.WriteLine(message);
    }
}