using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Speiser.Masterthesis.Analyzer.Analyzers;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ConfigurationService.Extensions;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Extensions;

namespace Speiser.Masterthesis.Analyzer
{
    public class AntiPatternAnalyzer
    {
        private readonly Dictionary<string, Type> registeredAnalyzers;
        private readonly string configPath;
        private readonly IServiceProvider serviceProvider;

        public AntiPatternAnalyzer(string configPath)
        {
            this.configPath = configPath;
            this.registeredAnalyzers = this.GetAllAnalyzers();
            this.serviceProvider = this.CreateServiceProvider();
        }

        public Task<Report> Run()
        {
            var config = this.GetConfiguration();
            var analyzers = this.GetAnalyzers(config);
            var report = new Report();

            foreach (var project in config.Projects)
            {
                var services = this.GetServices(project);
                report.Findings.Add(project, analyzers.Select(analyzer => analyzer.Analyze(services, config)).ToArray());
            }

            return Task.FromResult(report);
        }

        private Dictionary<string, Type> GetAllAnalyzers()
        {
            var iface = typeof(IAnalyzer);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type => !type.IsInterface && iface.IsAssignableFrom(type))
                .ToDictionary(type => type.Name.Replace("Analyzer", string.Empty));
        }

        private Configuration GetConfiguration()
            => this.serviceProvider.GetService<IConfigurationService>()
                .GetConfiguration(this.configPath);

        private IEnumerable<IAnalyzer> GetAnalyzers(Configuration config)
        {
            var analyzers = new List<IAnalyzer>();

            foreach (var analyzerKey in config.Analyzers)
            {
                if (this.registeredAnalyzers.TryGetValue(analyzerKey, out var type))
                {
                    analyzers.Add((IAnalyzer)this.serviceProvider.GetService(type));
                }
                else
                {
                    throw new InvalidOperationException($"Invalid analyzer in config found: {analyzerKey}");
                }
            }
            
            return analyzers;
        }

        private IEnumerable<Service> GetServices(string project)
        {
            var extractor = this.serviceProvider.GetService<IServiceExtractorProvider>()
                .GetServiceExtractor(project);
            return extractor.GetServices(project);
        }

        private IServiceProvider CreateServiceProvider()
            => new ServiceCollection()
                .RegisterConfigurationService()
                .RegisterServiceExtractorFactory()
                .RegisterSourceCodeService()
                .RegisterAnalyzers(this.registeredAnalyzers.Values)
                .BuildServiceProvider();
    }
}
