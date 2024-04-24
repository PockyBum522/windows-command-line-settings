using Serilog;

namespace WindowsCommandLineSettings.WindowsChangers;

public interface IWindowsChanger
{
    /// <summary>
    /// The command, as a string, that should be used to invoke this setting's change from the CLI.
    ///
    /// This should always be: public string InvocationCommand => GetType().Name;
    /// </summary>
    public string InvocationCommand => GetType().Name;
    
    /// <summary>
    /// The ILogger that will get passed in when Initialize() is called.
    ///
    /// Public so that it can automatically be checked that it is not null before use.
    /// </summary>
    public ILogger? Logger { get; }
    
    /// <summary>
    /// The ArgumentUtilities that will get passed in when Initialize() is called.
    ///
    /// Public so that it can automatically be checked that it is not null before use.
    /// </summary>
    public ArgumentUtilities? ArgumentUtilities { get; }

    /// <summary>
    /// Initialize the logger and anything else the changer may need when it runs
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="argumentUtilities"></param>
    public void Initialize(ILogger logger, ArgumentUtilities argumentUtilities);

    /// <summary>
    /// The actual method that runs the action. Called when end user specifies a command matching InvocationCommand on the CLI
    ///
    /// You can throw away originalArguments if your command is just a command and doesn't take parameters
    ///
    /// The top two lines of this method should always be:
    ///
    /// if (Logger is null) throw new NullReferenceException();
    /// Logger.Information("Running {ClassName} - {ThisMethod}", InvocationCommand, System.Reflection.MethodBase.GetCurrentMethod()?.Name);
    /// </summary>
    public void RunAction(string[] originalArguments);
}
