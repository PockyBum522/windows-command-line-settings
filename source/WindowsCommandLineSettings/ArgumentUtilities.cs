namespace WindowsCommandLineSettings;

public class ArgumentUtilities
{
    /// <summary>
    /// Finds the parameter(s) specified directly after a certain argument. Reads until the end of line or the start of another argument, basically looking for '-'
    /// </summary>
    /// <param name="argumentToMatch">The full string of the argument to get the parameters for, usually InvocationCommand in a IWindowsChanger</param>
    /// <param name="originalArguments">The full string array of the original CLI arguments</param>
    /// <returns>Each parameter following argumentToMatch</returns>
    public string[] GetArgumentParameters(string argumentToMatch, string[] originalArguments)
    {
        var matchedArgumentPosition = GetArgumentPosition(argumentToMatch, originalArguments);

        var returnParameters = new List<string>();
        
        for (var i = matchedArgumentPosition + 1; i < originalArguments.Length; i++)
        {
            var thisArgument = originalArguments[i];

            var argumentLeadingCharacter = thisArgument[0];

            if (argumentLeadingCharacter == '-')
            {
                return returnParameters.ToArray();
            }
            
            returnParameters.Add(thisArgument);
        }

        return returnParameters.ToArray();
    }

    /// <summary>
    /// Remove - and . and make lowercase 
    /// </summary>
    /// <param name="originalArgument">The original, raw argument passed by the end user</param>
    /// <returns>Argument formatted to the above specifications</returns>
    public string FormatArgumentForMatching(string originalArgument)
    {
        // Allow for formatting chars in argument. We remove them so that things like TaskbarSearchBarSetHidden
        // and Taskbar-Search-Bar-Set-Hidden all work
        var trimmedArgument = originalArgument.Trim();
        
        trimmedArgument = trimmedArgument.Replace("-", "");
        trimmedArgument = trimmedArgument.Replace(".", "");

        var replacedHyphenArgument = '-' + trimmedArgument;
        
        return replacedHyphenArgument.ToLower();
    }
    
    private int GetArgumentPosition(string argumentToMatch, string[] originalArguments)
    {
        for (var i = 0; i < originalArguments.Length; i++)
        {
            var formattedMatchArgument = FormatArgumentForMatching(argumentToMatch);
            var formattedCliArgument = FormatArgumentForMatching(originalArguments[i]);

            if (formattedMatchArgument == formattedCliArgument)
                return i;
        }

        return -1;
    }
}