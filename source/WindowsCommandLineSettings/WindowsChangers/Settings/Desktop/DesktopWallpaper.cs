using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32;
using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers.Settings.Desktop;

public class DesktopWallpaper : IWindowsChanger
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

        if (suppliedParameters.First() == "SetStretchedWallpaper")
        {
            var wallpaperPath = suppliedParameters[1];
            SetStretchedWallpaper(wallpaperPath);
        }
    }

    [SupportedOSPlatform("Windows7.0")]
    private void SetStretchedWallpaper(string wallpaperPath)
    {
        if (Logger is null) throw new NullReferenceException();
        if (ArgumentUtilities is null) throw new NullReferenceException();
        
        Logger.Information("Running {ThisName}", System.Reflection.MethodBase.GetCurrentMethod()?.Name);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        var wallpaperWorkFolder = Path.Join(@"C:\",
            "Users",
            "Public",
            "Documents",
            "Wallpapers");

        Directory.CreateDirectory(wallpaperWorkFolder);

        var wallpaperFilenameWithExtension = Path.GetFileName(wallpaperPath);
        var wallpaperFilenameOnly = Path.GetFileNameWithoutExtension(wallpaperPath);

        var newWallpaperFullPath = Path.Join(wallpaperWorkFolder, wallpaperFilenameWithExtension);
            
        File.Copy(wallpaperPath, newWallpaperFullPath, true);
        
        using var fs = File.Open(newWallpaperFullPath, FileMode.Open);
        
        var img = System.Drawing.Image.FromStream(fs);
        var newBmpPath = Path.Combine(wallpaperWorkFolder, $"{wallpaperFilenameOnly}.bmp");
        img.Save(newBmpPath, System.Drawing.Imaging.ImageFormat.Bmp);

        var key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

        if (key is null) throw new NullReferenceException();
        
        key.SetValue(@"WallpaperStyle", 22.ToString());
        key.SetValue(@"TileWallpaper", 0.ToString());
        
        SystemParametersInfo(20, 0, newBmpPath, 0x01 | 0x02);
    }
}
