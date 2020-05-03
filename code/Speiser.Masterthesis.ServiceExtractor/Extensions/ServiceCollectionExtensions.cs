using Microsoft.Extensions.DependencyInjection;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Internals;

namespace Speiser.Masterthesis.ServiceExtractor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServiceExtractorFactory(this IServiceCollection services)
            => services
                .AddSingleton<IServiceExtractor, DockerComposeServiceExtractor>()
                .AddSingleton<IServiceExtractorProvider, ServiceExtractorProvider>();
    }
}
