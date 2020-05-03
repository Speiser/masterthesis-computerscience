using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts.Syntax;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    internal class MonitoringAnalyzer : IAnalyzer
    {
        private readonly ISourceCodeService sourceCodeService;

        public MonitoringAnalyzer(ISourceCodeService sourceCodeService)
        {
            this.sourceCodeService = sourceCodeService;
        }

        public Finding Analyze(IEnumerable<Service> services, Configuration _)
        {
            // Note: The implementation can lead to false positives!
            //       In case that the setup call is extracted to another
            //       location and just called from Configure(Services).
            //       Should be fine for the prototype!
            var messages = new List<string>();
            var servicesWithSource = services.GetServicesWithSource().GetDotnetProjects(this.sourceCodeService);
            var projectCount = 0;

            foreach (var serviceWithSource in servicesWithSource)
            {
                var source = this.sourceCodeService.GetParsedSource(serviceWithSource.ProjectPath);
                var startup = source.GetClass("Startup");
                
                // Ignore projects without startup
                if (startup is null)
                    continue;

                projectCount++;
                var configureServicesMethod = startup.GetMethod("ConfigureServices");
                if (HasAddHealthChecks(configureServicesMethod))
                    continue;

                var configureMethod = startup.GetMethod("Configure");
                if (HasMapHealthChecks(configureMethod))
                    continue;

                messages.Add($"No healthcheck in {serviceWithSource.Name}");
            }

            return this.DefaultFindingFactory(messages, projectCount);
        }

        private static bool HasAddHealthChecks(CodeMethod method)
            => StatementContains(method, ".AddHealthChecks(");

        private static bool HasMapHealthChecks(CodeMethod method)
            => StatementContains(method, ".MapHealthChecks(");

        private static bool StatementContains(CodeMethod method, string value)
            => method.Statements.Any(s => s.Statement.Contains(value));
    }
}
