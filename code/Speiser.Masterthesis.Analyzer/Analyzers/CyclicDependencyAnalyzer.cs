using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    internal class CyclicDependencyAnalyzer : IAnalyzer
    {
        public Finding Analyze(IEnumerable<Service> services, Configuration _)
        {
            var messages = new List<string>();
            foreach (var service in services)
            {
                if (!HasCyclicDependency(service))
                    continue;

                messages.Add($"{service.Name} has a cyclic dependency.");
            }

            return this.DefaultFindingFactory(messages, services.Count());
        }

        private static bool HasCyclicDependency(Service service)
        {
            var visited = new List<Service>();
            return service.DependsOn
                .Any(s => CyclicDependencyHelper(s, service, visited));
        }

        private static bool CyclicDependencyHelper(Service service, Service original, List<Service> visited)
        {
            visited.Add(service);
            if (service.Name == original.Name) return true;
            return service.DependsOn
                .Where(x => !visited.Contains(x))
                .Any(s => CyclicDependencyHelper(s, original, visited));
        }
    }
}
