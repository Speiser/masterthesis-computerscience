using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.Analyzer.Analyzers;
using Speiser.Masterthesis.Analyzer.Contracts;

namespace Speiser.Masterthesis.Analyzer.Extensions
{
    internal static class AnalyzerExtensions
    {
        public static string GetAnalyzerName(this IAnalyzer analyzer)
            => analyzer.GetType().Name.Replace("Analyzer", string.Empty);

        public static Finding DefaultFindingFactory(
            this IAnalyzer analyzer,
            IEnumerable<string> messages,
            int divisor)
        {
            var messagesCount = messages.Count();
            return new Finding
            {
                ViolationCount = messagesCount,
                Analyzer = analyzer.GetAnalyzerName(),
                Messages = messages,
                RelativeViolationCount = (decimal)messagesCount / divisor
            };
        }
    }
}
