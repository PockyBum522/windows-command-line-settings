using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Themes;

public class WindowTransparency : IWindowsChanger
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
        
        Logger.Information("Running {ClassName} - {ThisMethod} (Parameters are: {Parameters})", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name, @suppliedParameters);

        var suppliedParameter = suppliedParameters[0];
        SetWindowTransparencyToDisabled(suppliedParameter);
    }

    [SupportedOSPlatform("Windows7.0")]
    private void SetWindowTransparencyToDisabled(string suppliedParameter)
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);

        if (key == null) throw new NullReferenceException();

        key.SetValue("EnableTransparency", int.Parse(suppliedParameter));
    }
}
