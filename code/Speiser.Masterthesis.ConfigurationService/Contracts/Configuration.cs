namespace Speiser.Masterthesis.ConfigurationService.Contracts
{
    public class Configuration
    {
        public string[] Projects { get; set; }
        public string[] Analyzers { get; set; }
        public string[] IgnoredLibraryPrefix { get; set; }
    }
}
