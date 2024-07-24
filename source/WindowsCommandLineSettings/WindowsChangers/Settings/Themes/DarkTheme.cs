using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Themes;

public class DarkTheme : IWindowsChanger
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
    /// Sets the monitor timeout to be X supplied number of minutes 
    /// </summary>
    [SupportedOSPlatform("Windows7.0")]
    public void RunAction(string[] originalArguments)
    {
        if (Logger is null) throw new NullReferenceException();
        if (ArgumentUtilities is null) throw new NullReferenceException();
        
        var suppliedParameters = ArgumentUtilities.GetArgumentParameters(InvocationCommand, originalArguments);
        
        Logger.Information("Running {ClassName} - {ThisMethod}", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name, @suppliedParameters);

        SetThemeToDark();
    }

    [SupportedOSPlatform("Windows7.0")]
    private void SetThemeToDark()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("AppsUseLightTheme", 0);
        key.SetValue("SystemUsesLightTheme", 0);

        using var accentKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", true);

        if (accentKey == null) throw new NullReferenceException();
        
        var paletteData = new byte[] { 0x9B, 0x9A, 0x99, 0x00, 0x84, 0x83, 0x81, 0x00, 0x6D, 0x6B, 0x6A, 0x00, 0x4C, 0x4A, 0x48, 0x00, 0x36, 0x35, 0x33, 0x00, 0x26, 0x25, 0x24, 0x00, 0x19, 0x19, 0x19, 0x00, 0x10, 0x7C, 0x10, 0x00  };
        
        accentKey.SetValue("AccentColorMenu", 4282927692);
        accentKey.SetValue("AccentPalette", paletteData);
        accentKey.SetValue("StartColorMenu", 4281546038);
        
        using var dwmKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\DWM", true);
  
        if (dwmKey == null) throw new NullReferenceException();
        
        dwmKey.SetValue("AccentColor", 4282927692);        
        dwmKey.SetValue("ColorizationAfterglow", 3293334088);
        dwmKey.SetValue("ColorizationColor", 3293334088);
    }
}
