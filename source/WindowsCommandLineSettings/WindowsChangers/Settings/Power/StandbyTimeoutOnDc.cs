﻿using System.Diagnostics;
using System.Runtime.Versioning;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Power;

public class StandbyTimeoutOnDc : IWindowsChanger
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

        var timeoutMinutes = suppliedParameters[0];
        SetStandbyTimeoutDcMinutes(timeoutMinutes);
    }

    [SupportedOSPlatform("Windows7.0")]
    private void SetStandbyTimeoutDcMinutes(string timeoutMinutes)
    {
        var processStartInfo = new ProcessStartInfo()
        {
            FileName = "powercfg.exe",
            Arguments = $"-x -standby-timeout-ac {timeoutMinutes}",
            UseShellExecute = true
        };

        Process.Start(processStartInfo)?.WaitForExit();
    }
}
