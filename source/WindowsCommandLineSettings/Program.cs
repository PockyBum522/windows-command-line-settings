using Serilog;
using WindowsCommandLineSettings.WindowsChangers;
using WindowsCommandLineSettings.WindowsChangers.Settings.Desktop;
using WindowsCommandLineSettings.WindowsChangers.Settings.Power;
using WindowsCommandLineSettings.WindowsChangers.Settings.Taskbar;
using WindowsCommandLineSettings.WindowsChangers.Settings.Themes;

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
            // Taskbar
            new TaskbarSearchBar(),         
            
            // Desktop
            new DesktopWallpaper(),         
            new DesktopIcons(),
            
            // Power
            new DiskTimeoutOnAc(),
            new DiskTimeoutOnDc(),
            new HibernateTimeoutOnAc(),
            new HibernateTimeoutOnDc(),
            new MonitorTimeoutOnAc(),
            new MonitorTimeoutOnDc(),
            new StandbyTimeoutOnAc(),
            new StandbyTimeoutOnDc(),
            
            // Themes
            new WindowTransparency(),
            new DarkTheme(),
            new DarkWindowTitlebars()
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

                if (trimmedCommandToMatch != trimmedCliArgument) continue;
                
                // Match!
                settingsChanger.RunAction(cliArguments);
                return;
            }
        }

        Console.WriteLine("Could not find specified argument. Check the help with /? and verify spelling");
    }

    public static string TestedCommandsMessage => @"
Tested Examples:
(v0.0.03)

----------==================== Taskbar ====================----------
(Can be run as: ADMIN or USER)

WindowsCommandLineSettings.exe -TaskbarSearchBar SetHidden
WindowsCommandLineSettings.exe -TaskbarSearchBar SetIcon


----------==================== Desktop ====================----------
(Can be run as: ADMIN or USER)

WindowsCommandLineSettings.exe -DesktopWallpaper SetStretchedWallpaper C:\Windows\Web\Wallpaper\Theme1\img13.jpg
 
WindowsCommandLineSettings.exe -DesktopIcons DeleteAllFilesWithExtension *.txt
WindowsCommandLineSettings.exe -DesktopIcons DeleteAllFilesWithExtension *.lnk


----------==================== Power Timeouts ====================----------
(Can be run as: ADMIN or USER)

Supplied parameter is the number of minutes to wait before activating the power saving measure, 0 to disable:

WindowsCommandLineSettings.exe -DiskTimeoutOnAc 0
WindowsCommandLineSettings.exe -DiskTimeoutOnDc 5

WindowsCommandLineSettings.exe -HibernateTimeoutOnAc 0
WindowsCommandLineSettings.exe -HibernateTimeoutOnDc 5

WindowsCommandLineSettings.exe -MonitorTimeoutOnAc 0
WindowsCommandLineSettings.exe -MonitorTimeoutOnDc 5

WindowsCommandLineSettings.exe -StandbyTimeoutOnAc 0
WindowsCommandLineSettings.exe -StandbyTimeoutOnDc 5


----------==================== Themes ====================----------
(Can be run as: ADMIN or USER)

WindowsCommandLineSettings.exe -WindowTransparency 0
WindowsCommandLineSettings.exe -WindowTransparency 1

WindowsCommandLineSettings.exe -DarkTheme

WindowsCommandLineSettings.exe -DarkWindowTitlebars


";
}

