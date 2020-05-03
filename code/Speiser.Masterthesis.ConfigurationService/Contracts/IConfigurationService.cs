namespace Speiser.Masterthesis.ConfigurationService.Contracts
{
    public interface IConfigurationService
    {
        Configuration GetConfiguration(string configPath);
    }
}
