using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Taskbar;

public class TaskbarSearchBarCollapseToIcon : IWindowsChanger
{
    public string InvocationCommand => GetType().Name;

    public ILogger? Logger { get; private set; }
    
    public void Initialize(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Collapses the search in taskbar to completely hidden 
    /// </summary>
    /// <exception cref="NullReferenceException">Throws on registry access error</exception>
    [SupportedOSPlatform("Windows7.0")]
    public void RunAction()
    {
        if (Logger is null) throw new NullReferenceException();
        
        Logger.Information("Running {ClassName} - {ThisMethod}", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", true);
        
        if (key == null) throw new NullReferenceException();
        
        key.SetValue("SearchboxTaskbarMode", 1);
    }
}
