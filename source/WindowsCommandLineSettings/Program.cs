﻿using Serilog;
using WindowsCommandLineSettings.WindowsChangers;
using WindowsCommandLineSettings.WindowsChangers.Settings.Taskbar;

namespace WindowsCommandLineSettings;

// In the directory with the .csproj, run:
// dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true 

internal static class Program
{
    // ReSharper disable InconsistentNaming
    private static readonly ILogger _logger = DependencyBuilders.BuildLogger(ApplicationPaths.LogPath);
    private static readonly HelpManager _helpManager = new(_logger);
    private static readonly ArgumentUtilities _argumentUtilities = new();
    // ReSharper restore InconsistentNaming
    
    public static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            _helpManager.PrintHelpToConsole();
            return;
        }

        if (args.Contains("-help"))
        {
            _helpManager.FetchAppropriateHelpMessage(args);
        }

        var changers = new List<IWindowsChanger>
        {
            // ----------==================== Taskbar Changers ====================----------
            
            // Taskbar - Search bar changer
            new TaskbarSearchBar(), // Options: SetHidden, SetIcon
        };

        InitializeAllChangers(changers);
        
        RunChangerOnMatchingCommand(args, changers);
    }

    private static void InitializeAllChangers(List<IWindowsChanger> changers)
    {
        foreach (var settingsChanger in changers)
        {
            settingsChanger.Initialize(_logger, _argumentUtilities);

            if (settingsChanger.Logger is null) throw new NullReferenceException();
            if (settingsChanger.ArgumentUtilities is null) throw new NullReferenceException();
        }
    }

    private static void RunChangerOnMatchingCommand(string[] cliArguments, List<IWindowsChanger> changers)
    {
        foreach (var settingsChanger in changers)
        {
            var trimmedCommandToMatch = _argumentUtilities.FormatArgumentForMatching(settingsChanger.InvocationCommand); 
             
            foreach (var cliArgument in cliArguments)
            {
                var trimmedCliArgument = _argumentUtilities.FormatArgumentForMatching(cliArgument);
                
                if (trimmedCommandToMatch == trimmedCliArgument)
                {
                    // Match!
                    settingsChanger.RunAction(cliArguments);
                }
            }
        }
    }
}
