using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Taskbar;

public class TaskbarSearchBar : IWindowsChanger
{
    
    public string InvocationCommand => GetType().Name;

    public ILogger? Logger { get; private set; }
    public ArgumentUtilities? ArgumentUtilities { get; private set; }
    
    public void Initialize(ILogger logger, ArgumentUtilities argumentUtilities)
    {
        ArgumentUtilities = argumentUtilities;
        Logger = logger;
    }

    /// <summary>
    /// Collapses the search in taskbar to completely hidden 
    /// </summary>
    /// <exception cref="NullReferenceException">Throws on registry access error</exception>
    [SupportedOSPlatform("Windows7.0")]
    public void RunAction(string[] originalArguments)
    {
        if (Logger is null) throw new NullReferenceException();
        if (ArgumentUtilities is null) throw new NullReferenceException();
        
        var trimmedArgumentToMatch = ArgumentUtilities.FormatArgumentForMatching(InvocationCommand);
        var suppliedParameters = ArgumentUtilities.GetArgumentParameters(InvocationCommand, originalArguments);
        
        Logger.Information("Running {ClassName} - {ThisMethod} (Parameters are: {Parameters})", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name, @suppliedParameters);

        if (suppliedParameters.First() == "SetHidden")
            HideTaskBarSearchBar();

        if (suppliedParameters.First() == "SetIcon")
            IconifyTaskBarSearchBar();
    }

    [SupportedOSPlatform("Windows7.0")]
    private void HideTaskBarSearchBar()
    {
        if (Logger is null) throw new NullReferenceException();

        Logger.Information("Setting the taskbar search bar to hidden");

        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
        
        if (key == null) throw new NullReferenceException();
        
        key.SetValue("SearchboxTaskbarMode", 0);
    }

    [SupportedOSPlatform("Windows7.0")]
    private void IconifyTaskBarSearchBar()
    {
        if (Logger is null) throw new NullReferenceException();
        
        Logger.Information("Setting the taskbar search bar to icon only");
        
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("SearchboxTaskbarMode", 1);
    }
}
