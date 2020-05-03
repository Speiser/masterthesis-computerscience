namespace Speiser.Masterthesis.ServiceExtractor.Contracts
{
    public interface IServiceExtractorProvider
    {
        IServiceExtractor GetServiceExtractor(string projectPath);
    }
}
