using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    internal class HardcodedEndpointsAnalyzer : IAnalyzer
    {
        // Based on: https://superuser.com/questions/623168/regex-to-parse-urls-from-text
        private static readonly Regex urlMatcher = new Regex(
            @"(https?):\/\/(www\.)?[a-z0-9\.:].*?(?=\s)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // Based on: https://www.regular-expressions.info/ip.html
        private static readonly Regex ipMatcher = new Regex(
            @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ISourceCodeService sourceCodeService;

        public HardcodedEndpointsAnalyzer(ISourceCodeService sourceCodeService)
        {
            this.sourceCodeService = sourceCodeService;
        }

        public Finding Analyze(IEnumerable<Service> services, Configuration _)
        {
            var messages = new List<string>();
            var servicesWithSource = services.GetServicesWithSource()
                .GetDotnetProjects(this.sourceCodeService)
                .ToArray();

            var violationCount = 0;

            foreach (var serviceWithSource in servicesWithSource)
            {
                var fileInfos = this.sourceCodeService.GetConfigurationFiles(serviceWithSource.ProjectPath);
                var endpoints = new HashSet<string>();

                foreach (var fileInfo in fileInfos)
                {
                    var urlMatches = urlMatcher.Matches(fileInfo.Content);
                    var ipMatches = ipMatcher.Matches(fileInfo.Content);
                    if (urlMatches.Count == 0 && ipMatches.Count == 0)
                        continue;

                    var matchesBuilder = new StringBuilder();
                    matchesBuilder
                        .AppendLine($"{fileInfo.Path} contains hardcoded endpoints:");

                    foreach (var match in urlMatches.Concat(ipMatches))
                    {
                        endpoints.Add(match.Value);
                        matchesBuilder.AppendLine($"  {match.Value}");
                    }
                    messages.Add(matchesBuilder.ToString());
                }

                violationCount += endpoints.Count;
            }

            return new Finding
            {
                ViolationCount = violationCount,
                Analyzer = this.GetAnalyzerName(),
                Messages = messages,
                RelativeViolationCount = (decimal)violationCount / servicesWithSource.Length
            };
        }
    }
}
