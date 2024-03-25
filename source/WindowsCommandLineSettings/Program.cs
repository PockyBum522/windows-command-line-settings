namespace WindowsCommandLineSettings;

internal static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintHelpToConsole();
            
            return;
        }
        
        Console.WriteLine("Hello, World!");
    }

    private static void PrintHelpToConsole()
    {
        var message = @"
This help is printed when windowsCommandLineSettings.exe is run with no arguments or with -help

To print this help:
-help

To see available settings that can be changed:
-commands

To get extended help on a specific setting change:
-help setting-name

To explicitly set log file location (Timestamp will be appended to log name automatically):
-logpath ""C:\Users\Public\Documents\Logs\WindowsCommandLineSettings\log_.log"" 
Note: The default log path is the above path";

        Console.WriteLine();
    }
}