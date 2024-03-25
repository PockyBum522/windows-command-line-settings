namespace WindowsCommandLineSettings
{
    /// <summary>
    /// Contains the few paths for this application that must be hardcoded
    /// </summary>
    public static class ApplicationPaths
    {
        /// <summary>
        /// Per-user log folder path
        /// </summary>
        private static string LogAppBasePath =>
            Path.Combine(
                "C:",
                "Users",
                "Public",
                "Documents",
                "Logs",
                "WindowsCommandLineSettings");

        /// <summary>
        /// Actual log file path passed to the ILogger configuration
        /// </summary>
        public static string LogPath =>
            Path.Combine(
                LogAppBasePath,
                "log_.log");
        
        /// <summary>
        /// The directory the assembly is running from
        /// </summary>
        public static string ThisApplicationRunFromDirectoryPath => 
            Path.GetDirectoryName(Environment.ProcessPath) ?? "";

        /// <summary>
        /// The top level dir, useful for getting to configuration folders and resource folders
        /// This is the directory the bootstrapper bat file is in
        /// </summary>
        public static string SetupAssistantRootDir => 
            Path.GetFullPath(
                Path.Join(
                    ThisApplicationRunFromDirectoryPath, "../../../../.."));
        
        /// <summary>
        /// The full path to this application's running assembly
        /// </summary>
        public static string ThisApplicationProcessPath => 
            Environment.ProcessPath ?? "";

        /// <summary>
        /// Where to put the JSON file representing what state the setup is in, state is based on user selection in
        /// MainWindow
        /// </summary>
        public static string StatePath => @"C:\Users\Public\Documents\state.json";

        /// <summary>
        /// Path to the file containing registry files and ps1 scripts
        /// </summary>
        public static string ResourceDirectoryExternalFiles =>
            Path.GetFullPath(
                Path.Join(
                    SetupAssistantRootDir,
                    "resources"));

    }
}
