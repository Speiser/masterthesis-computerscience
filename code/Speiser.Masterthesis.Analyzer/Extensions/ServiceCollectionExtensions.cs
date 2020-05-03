using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Speiser.Masterthesis.Analyzer.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterAnalyzers(
            this IServiceCollection services,
            IEnumerable<Type> registeredAnalyzers)
        {
            foreach (var registeredAnalyzer in registeredAnalyzers)
            {
                services.AddSingleton(registeredAnalyzer);
            }

            return services;
        }
    }
}
