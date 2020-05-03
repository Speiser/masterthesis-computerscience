using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.Analyzer.Contracts;

namespace Speiser.Masterthesis.Analyzer.Extensions
{
    public static class ReportExtensions
    {
        public static Report ScaleRelativeValues(this Report report)
        {
            var highest = GetHighestPerAnalyzer(report);
            var scalingFactors = GetScalingFactorPerAnalyzer(report, highest);

            foreach (var project in report.Findings)
            {
                foreach (var finding in project.Value)
                {
                    finding.ScaledRelativeViolationCount =
                        scalingFactors[finding.Analyzer] * finding.RelativeViolationCount;
                }
            }
            
            return report;
        }

        private static Dictionary<string, decimal> GetPrefilledDictionary(Report report)
        {
            var dict = new Dictionary<string, decimal>();

            // Prefill dictionary
            foreach (var finding in report.Findings.First().Value)
            {
                dict.Add(finding.Analyzer, 0);
            }

            return dict;
        }

        private static Dictionary<string, decimal> GetHighestPerAnalyzer(Report report)
        {
            // Get highest per analyzer (over all projects)
            var highest = GetPrefilledDictionary(report);

            foreach (var project in report.Findings)
            {
                foreach (var finding in project.Value)
                {
                    if (highest[finding.Analyzer] < finding.RelativeViolationCount)
                    {
                        highest[finding.Analyzer] = finding.RelativeViolationCount;
                    }
                }
            }

            return highest;
        }

        private static Dictionary<string, decimal> GetScalingFactorPerAnalyzer(
            Report report, Dictionary<string, decimal> highest)
        {
            var scalingFactor = GetPrefilledDictionary(report);
            foreach (var kvp in highest)
            {
                scalingFactor[kvp.Key] = kvp.Value == 0
                    ? 1
                    : 1 / kvp.Value;
            }
            return scalingFactor;
        }
    }
}
