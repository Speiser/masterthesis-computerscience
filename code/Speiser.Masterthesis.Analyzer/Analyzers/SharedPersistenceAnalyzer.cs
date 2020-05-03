using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    internal class SharedPersistenceAnalyzer : IAnalyzer
    {
        private readonly ISourceCodeService sourceCodeService;
        private readonly IProjectFileReader projectFileReader;

        public SharedPersistenceAnalyzer(
            ISourceCodeService sourceCodeService,
            IProjectFileReader projectFileReader)
        {
            this.sourceCodeService = sourceCodeService;
            this.projectFileReader = projectFileReader;
        }

        public Finding Analyze(IEnumerable<Service> services, Configuration _)
        {
            var messages = new List<string>();
            var servicesWithSource = services.GetServicesWithSource()
                .GetDotnetProjects(this.sourceCodeService)
                .ToArray();
            var foundConnectionStrings = new HashSet<(string, string)>();

            foreach (var serviceWithSource in servicesWithSource)
            {
                if (!serviceWithSource.IsAspNetProject(this.sourceCodeService, this.projectFileReader))
                    continue;

                var fileInfos = this.sourceCodeService.GetConfigurationFiles(serviceWithSource.ProjectPath);

                foreach (var fileInfo in fileInfos)
                {
                    if (!fileInfo.Path.ToLower().Contains("appsettings"))
                        continue;

                    var appSettings = JsonConvert.DeserializeObject<AppSettings>(fileInfo.Content);
                    if (!string.IsNullOrWhiteSpace(appSettings.ConnectionString))
                        foundConnectionStrings.Add((serviceWithSource.Name, appSettings.ConnectionString));

                    if (appSettings.ConnectionStrings != null)
                    {
                        foreach (var connectionString in appSettings.ConnectionStrings)
                        {
                            foundConnectionStrings.Add((serviceWithSource.Name, connectionString.Value));
                        }
                    }
                }
            }

            // Find duplicates: https://stackoverflow.com/a/18547390
            var multipleUsedConnectionsStrings = foundConnectionStrings
                .Select(fcs => fcs.Item2)
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(x => x.Key)
                .ToArray();

            foreach (var connectionString in multipleUsedConnectionsStrings)
            {
                messages.Add($"Connection string {connectionString} is used multiple times");
            }

            return new Finding
            {
                ViolationCount = multipleUsedConnectionsStrings.Length,
                Analyzer = this.GetAnalyzerName(),
                Messages = messages,
                RelativeViolationCount = (decimal)multipleUsedConnectionsStrings.Length / servicesWithSource.Length
            };
        }

        private class AppSettings
        {
            public string ConnectionString { get; set; }
            public Dictionary<string, string> ConnectionStrings { get; set; }
        }
    }
}
