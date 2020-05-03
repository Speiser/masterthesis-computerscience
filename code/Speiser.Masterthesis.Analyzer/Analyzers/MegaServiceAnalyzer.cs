using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    internal class MegaServiceAnalyzer : IAnalyzer
    {
        private const int ControllerThresholdPerService = 3;
        private readonly ISourceCodeService sourceCodeService;

        public MegaServiceAnalyzer(ISourceCodeService sourceCodeService)
        {
            this.sourceCodeService = sourceCodeService;
        }

        public Finding Analyze(IEnumerable<Service> services, Configuration _)
        {
            // Note: This can lead to false positives,
            // since versioned controllers are not considered!
            var messages = new List<string>();
            var servicesWithSource = services.GetServicesWithSource()
                .GetDotnetProjects(this.sourceCodeService)
                .ToArray();

            foreach (var serviceWithSource in servicesWithSource)
            {
                var source = this.sourceCodeService.GetParsedSource(serviceWithSource.ProjectPath);
                if (source.GetControllers().Count() > ControllerThresholdPerService)
                {
                    messages.Add($"Service {serviceWithSource.ProjectPath} contains more than {ControllerThresholdPerService} controllers.");
                }
            }

            return this.DefaultFindingFactory(messages, servicesWithSource.Length);
        }
    }
}
