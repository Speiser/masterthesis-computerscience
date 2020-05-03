using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    public class SharedLibraryAnalyzer : IAnalyzer
    {
        private readonly ISourceCodeService sourceCodeService;
        private readonly IProjectFileReader projectFileReader;

        private readonly IEnumerable<string> DefaultIgnoredLibraryPrefix = new[]
        {
            "System.",
            "Microsoft.",
            "AspNetCore."
        };

        public SharedLibraryAnalyzer(
            ISourceCodeService sourceCodeService,
            IProjectFileReader projectFileReader)
        {
            this.sourceCodeService = sourceCodeService;
            this.projectFileReader = projectFileReader;
        }

        public Finding Analyze(IEnumerable<Service> services, Configuration config)
        {
            var servicesWithSource = services.GetServicesWithSource()
                .GetDotnetProjects(this.sourceCodeService)
                // Filter projects that are referenced by multiple services in the docker compose file
                .Distinct(ServiceComparer.Default)
                .ToArray();
            var usedLibraries = new Dictionary<string, int>();

            foreach (var service in servicesWithSource)
            {
                var projectFilePath = this.sourceCodeService.GetProjectFile(service.ProjectPath);
                var projectFile = this.projectFileReader.ParseProjectFile(projectFilePath);
                projectFile = this.FilterIgnoredLibraries(projectFile, config.IgnoredLibraryPrefix);
                foreach (var library in projectFile.Libraries)
                {
                    if (usedLibraries.ContainsKey(library))
                    {
                        usedLibraries[library]++;
                    }
                    else
                    {
                        usedLibraries.Add(library, 1);
                    }
                }
            }

            var sharedLibraries = usedLibraries.Where(kv => kv.Value > 1).ToArray();
            var messages = sharedLibraries.Select(kv => $"Library {kv.Key} was used in {kv.Value} different projects.");

            return new Finding
            {
                ViolationCount = messages.Count(),
                Analyzer = this.GetAnalyzerName(),
                Messages = messages,
                RelativeViolationCount = (decimal)sharedLibraries.Length / servicesWithSource.Length
            };
        }

        private ProjectFile FilterIgnoredLibraries(ProjectFile projectFile, string[] userIgnoredLibraryPrefix)
        {
            var ignoredPrefixes = this.DefaultIgnoredLibraryPrefix;
            if (userIgnoredLibraryPrefix != null)
            {
                ignoredPrefixes = ignoredPrefixes.Concat(userIgnoredLibraryPrefix);
            }

            projectFile.Libraries = projectFile.Libraries.Where(lib =>
            {
                foreach (var ignored in ignoredPrefixes)
                {
                    if (lib.StartsWith(ignored))
                        return false;
                }

                return true;
            });
            return projectFile;
        }

        private class ServiceComparer : IEqualityComparer<Service>
        {
            static ServiceComparer()
            {
                Default = new ServiceComparer();
            }

            public static IEqualityComparer<Service> Default { get; }
            public bool Equals(Service x, Service y) => x.ProjectPath == y.ProjectPath;
            public int GetHashCode(Service obj) => -1;
        }
    }
}
