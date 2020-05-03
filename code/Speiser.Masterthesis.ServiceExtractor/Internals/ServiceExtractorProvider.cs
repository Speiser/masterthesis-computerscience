using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Speiser.Masterthesis.ServiceExtractor.Contracts;

namespace Speiser.Masterthesis.ServiceExtractor.Internals
{
    internal class ServiceExtractorProvider : IServiceExtractorProvider
    {
        private readonly IServiceProvider serviceProvider;

        public ServiceExtractorProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IServiceExtractor GetServiceExtractor(string projectPath)
            => this.serviceProvider
                .GetServices<IServiceExtractor>()
                .Single(se => se.IsUsedByProject(projectPath));
    }
}
