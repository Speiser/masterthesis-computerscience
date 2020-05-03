using Microsoft.Extensions.DependencyInjection;
using Speiser.Masterthesis.ConfigurationService.Contracts;

namespace Speiser.Masterthesis.ConfigurationService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterConfigurationService(this IServiceCollection services)
            => services.AddSingleton<IConfigurationService, ConfigurationReader>();
    }
}
