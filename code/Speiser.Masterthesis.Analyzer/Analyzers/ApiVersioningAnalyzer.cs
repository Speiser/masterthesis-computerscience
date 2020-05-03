using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    internal class ApiVersioningAnalyzer : IAnalyzer
    {
        private readonly ISourceCodeService sourceCodeService;

        public ApiVersioningAnalyzer(ISourceCodeService service)
        {
            this.sourceCodeService = service;
        }

        public Finding Analyze(IEnumerable<Service> services, Configuration _)
        {
            var messages = new List<string>();
            var controllerCount = 0;

            var servicesWithSource = services.GetServicesWithSource().GetDotnetProjects(this.sourceCodeService);

            foreach (var serviceWithSource in servicesWithSource)
            {
                var source = this.sourceCodeService.GetParsedSource(serviceWithSource.ProjectPath);

                // Get all controllers
                var controllers = source.GetControllers().ToArray();
                controllerCount += controllers.Length;

                foreach (var controller in controllers)
                {
                    // Check for ApiVersionAttribute on Controller
                    if (controller.Attributes.Any(a => a.Identifier == "ApiVersion"))
                        continue;

                    // Get RouteAttribute
                    if (controller.Attributes.GetAttribute("Route").ContainsVersionString())
                        continue;

                    // Check if ALL endpoints (methods with HttpGet, HttpPost, etc Attributes)
                    // have an Route Attribute containing a version string.
                    var endpoints = controller.GetEndpoints();

                    if (endpoints.Any() // This is a "quick" fix for controllers with endpoints (that dont have an attribute)..
                     && endpoints.All(endpoint => endpoint.Attributes.GetAttribute("Route").ContainsVersionString()))
                    {
                        continue;
                    }

                    messages.Add($"Controller {controller.Identifier} in {source.ProjectFile.Split('\\').Last()} does not contain versioning!");
                }
            }

            return this.DefaultFindingFactory(messages, controllerCount);
        }
    }
}
