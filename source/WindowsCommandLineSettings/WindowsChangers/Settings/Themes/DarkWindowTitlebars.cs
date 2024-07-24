using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Themes;

public class DarkWindowTitlebars : IWindowsChanger
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
        
        Logger.Information("Running {ClassName} - {ThisMethod}", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name);

        SetTitlebarsToDark();
    }

    [SupportedOSPlatform("Windows7.0")]
    private void SetTitlebarsToDark()
    {
        using var accentKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", true);
        
        if (accentKey == null) throw new NullReferenceException();

        // We have to do some really dumb stuff to get the number into a format SetValue will accept for a DWord
        accentKey.SetValue("StartColorMenu", BitConverter.ToInt32(BitConverter.GetBytes(0xff333536u), 0), RegistryValueKind.DWord);
        accentKey.SetValue("AccentColorMenu", BitConverter.ToInt32(BitConverter.GetBytes(0xff484a4cu), 0), RegistryValueKind.DWord);
        accentKey.SetValue("AccentPalette", new byte[]{ 0x9B, 0x9A, 0x99, 0x00, 0x84, 0x83, 0x81, 0x00, 0x6D, 0x6B, 0x6A, 0x00, 0x4C, 0x4A, 0x48, 0x00, 0x36, 0x35, 0x33, 0x00, 0x26, 0x25, 0x24, 0x00, 0x19, 0x19, 0x19, 0x00, 0x10, 0x7C, 0x10, 0x00 });

       
        using var dwmKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\DWM", true);
        
        if (dwmKey == null) throw new NullReferenceException();

        accentKey.SetValue("ColorizationAfterglow", BitConverter.ToInt32(BitConverter.GetBytes(0xc44c4a48u), 0), RegistryValueKind.DWord);
        accentKey.SetValue("ColorizationColor", BitConverter.ToInt32(BitConverter.GetBytes(0xc44c4a48u), 0), RegistryValueKind.DWord);
        accentKey.SetValue("AccentColor", BitConverter.ToInt32(BitConverter.GetBytes(0xff484a4cu), 0), RegistryValueKind.DWord);
    }
}
