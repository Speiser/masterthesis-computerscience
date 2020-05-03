using System.Collections.Generic;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.ConfigurationService.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Contracts;

namespace Speiser.Masterthesis.Analyzer.Analyzers
{
    public interface IAnalyzer
    {
        Finding Analyze(IEnumerable<Service> services, Configuration config);
    }
}
