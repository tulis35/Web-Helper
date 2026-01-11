namespace WebHelper.Models
{
    public class ExtAppSettings
    {
        public string Arguments { get; set; }
        public string AppPath { get; set; }
        public ExtAppSettings() 
        {
            Arguments = string.Empty;
            AppPath = string.Empty;
        }
        public ExtAppSettings(string arguments)
        {
            Arguments = arguments;
            AppPath = string.Empty;
        }
        public ExtAppSettings(string arguments, string appPath)
        {
            Arguments = arguments;
            AppPath = appPath;
        }
    }
}
