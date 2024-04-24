using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Desktop;

public class DesktopIcons : IWindowsChanger
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
    /// Sets a wallpaer to the path supplied in the parameter following "SetStretchedWallpaper"
    ///
    /// Original file should be a jpg, untested/unsure if other formats work 
    /// </summary>
    /// <exception cref="NullReferenceException">Throws on registry access error</exception>
    [SupportedOSPlatform("Windows7.0")]
    public void RunAction(string[] originalArguments)
    {
        if (Logger is null) throw new NullReferenceException();
        if (ArgumentUtilities is null) throw new NullReferenceException();
        
        var suppliedParameters = ArgumentUtilities.GetArgumentParameters(InvocationCommand, originalArguments);
        
        Logger.Information("Running {ClassName} - {ThisMethod} (Parameters are: {Parameters})", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name, @suppliedParameters);

        if (suppliedParameters.First() == "DeleteAllFilesWithExtension")
        {
            var extensionToMatch = suppliedParameters[1];
            DeleteDesktopFilesWithExtension(extensionToMatch);
        }
    }

    [SupportedOSPlatform("Windows7.0")]
    private void DeleteDesktopFilesWithExtension(string extensionsToDelete)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var publicDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

        extensionsToDelete = extensionsToDelete.Replace("*", "");
        
        DeleteAllFilesWithExtension(desktopPath, extensionsToDelete);
        DeleteAllFilesWithExtension(publicDesktopPath, extensionsToDelete);
    }
    
    private void DeleteAllFilesWithExtension(string pathToDeleteIn, string extensionToMatch)
    {
        var filesOnCommonDesktop = Directory.GetFiles(pathToDeleteIn); 
        
        foreach (var file in filesOnCommonDesktop)
        {
            if (file.EndsWith(extensionToMatch)) File.Delete(file);   
        }
    }
}
