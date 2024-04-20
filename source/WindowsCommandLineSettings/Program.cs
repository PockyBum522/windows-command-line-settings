using Serilog;
using WindowsCommandLineSettings.WindowsChangers;
using WindowsCommandLineSettings.WindowsChangers.Settings.Taskbar;

namespace WindowsCommandLineSettings;

// In the directory with the .csproj, run:
// dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true 

internal static class Program
{
    // ReSharper disable InconsistentNaming
    private static readonly ILogger _logger = DependencyBuilders.BuildLogger(ApplicationPaths.LogPath);
    private static readonly HelpManager HelpManager = new(_logger);
    // ReSharper restore InconsistentNaming
    
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            HelpManager.PrintHelpToConsole();
            return;
        }

        if (args.Contains("-help"))
        {
            HelpManager.FetchAppropriateHelpMessage(args);
        }

        var changers = new List<IWindowsChanger>
        {
            // ----------==================== Taskbar Changers ====================----------
            
            // Taskbar - Search icon changers:
            new TaskbarSearchBarSetHidden(),
            new TaskbarSearchBarCollapseToIcon()
        };

        InitializeAllChangers(changers);
        
        RunChangerOnMatchingCommand(args, changers);
    }

    private static void InitializeAllChangers(List<IWindowsChanger> changers)
    {
        foreach (var settingsChanger in changers)
        {
            settingsChanger.Initialize(_logger);

            if (settingsChanger.Logger is null) throw new NullReferenceException();
        }
    }

    private static void RunChangerOnMatchingCommand(string[] args, List<IWindowsChanger> changers)
    {
        foreach (var settingsChanger in changers)
        {
            var trimmedCommand = settingsChanger.InvocationCommand.ToLower();
            
            foreach (var argument in args)
            {
                var trimmedArgument = argument.ToLower().Trim();
                
                // Allow for formatting chars in argument. We remove them so that things like TaskbarSearchBarSetHidden
                // and Taskbar-Search-Bar-Set-Hidden all work
                trimmedArgument = trimmedArgument.Replace("-", "");
                trimmedArgument = trimmedArgument.Replace(".", "");

                if (trimmedCommand == trimmedArgument)
                {
                    // Match!
                    settingsChanger.RunAction();
                }
            }
        }
    }
}
