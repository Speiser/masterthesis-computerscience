using Microsoft.Extensions.DependencyInjection;
using Speiser.Masterthesis.SourceCodeService.Contracts;
using Speiser.Masterthesis.SourceCodeService.Internals;

namespace Speiser.Masterthesis.ServiceExtractor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterSourceCodeService(this IServiceCollection services)
            => services
                .AddSingleton<ISourceCodeService>(new CachedSourceCodeService(new SourceCodeService.Internals.SourceCodeService()))
                .AddSingleton<IProjectFileReader, ProjectFileReader>();
    }
}
