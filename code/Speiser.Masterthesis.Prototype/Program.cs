using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Speiser.Masterthesis.Analyzer;
using Speiser.Masterthesis.Analyzer.Contracts;
using Speiser.Masterthesis.Analyzer.Extensions;

namespace Speiser.Masterthesis.Prototype
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var report = (await new AntiPatternAnalyzer(args[0])
                .Run())
                .ScaleRelativeValues();
            var results = new List<ProjectResult>();

            foreach (var finding in report.Findings)
            {
                var result = finding.Value.TransformToProjectResult(finding.Key);
                results.Add(result);
            }

            var (rq1_weighted, rq1_notweighted) = GetRQ1Result(results);
            var (rq2_scaled, rq2_total, different) = GetRQ2Result(results);

            var output = new Results
            {
                RQ1_Weighted = rq1_weighted,
                RQ1_NotWeighted = rq1_notweighted,
                RQ2_Scaled = rq2_scaled,
                RQ2_Total = rq2_total,
                RQ2_Different = different,
                ProjectResults = results,
                RawResults = report
            };

            Console.WriteLine(JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        private static (Dictionary<string, decimal>, Dictionary<string, decimal>) GetRQ1Result(List<ProjectResult> results)
        {
            var weight = new AnalyzerResult
            {
                ApiVersioning = 6.377272727m,
                CyclicDependency = 7,
                HardcodedEndpoints = 8,
                Megaservice = 7,
                Monitoring = 5,
                SharedLibrary = 4,
                SharedPersistence = 6.377272727m
            };

            var withWeight = new Dictionary<string, decimal>(results.Count);

            foreach (var project in results)
            {
                var calculated =
                    project.Relative.ApiVersioning * weight.ApiVersioning
                  + project.Relative.CyclicDependency * weight.CyclicDependency
                  + project.Relative.HardcodedEndpoints * weight.HardcodedEndpoints
                  + project.Relative.Megaservice * weight.Megaservice
                  + project.Relative.Monitoring * weight.Monitoring
                  + project.Relative.SharedLibrary * weight.SharedLibrary
                  + project.Relative.SharedPersistence * weight.SharedPersistence;

                withWeight.Add(project.Project, calculated);
            }

            var withoutWeight = new Dictionary<string, decimal>(results.Count);

            foreach (var project in results)
            {
                var calculated =
                    project.Relative.ApiVersioning
                  + project.Relative.CyclicDependency
                  + project.Relative.HardcodedEndpoints
                  + project.Relative.Megaservice
                  + project.Relative.Monitoring
                  + project.Relative.SharedLibrary
                  + project.Relative.SharedPersistence;

                withoutWeight.Add(project.Project, calculated);
            }

            return (withWeight, withoutWeight);
        }

        private static (AnalyzerResult, AnalyzerResult, AnalyzerResult) GetRQ2Result(List<ProjectResult> results)
        {
            var scaled = new AnalyzerResult
            {
                ApiVersioning = results.Sum(x => x.Relative.ApiVersioning),
                CyclicDependency = results.Sum(x => x.Relative.CyclicDependency),
                HardcodedEndpoints = results.Sum(x => x.Relative.HardcodedEndpoints),
                Megaservice = results.Sum(x => x.Relative.Megaservice),
                Monitoring = results.Sum(x => x.Relative.Monitoring),
                SharedLibrary = results.Sum(x => x.Relative.SharedLibrary),
                SharedPersistence = results.Sum(x => x.Relative.SharedPersistence)
            };

            var total = new AnalyzerResult
            {
                ApiVersioning = results.Sum(x => x.Total.ApiVersioning),
                CyclicDependency = results.Sum(x => x.Total.CyclicDependency),
                HardcodedEndpoints = results.Sum(x => x.Total.HardcodedEndpoints),
                Megaservice = results.Sum(x => x.Total.Megaservice),
                Monitoring = results.Sum(x => x.Total.Monitoring),
                SharedLibrary = results.Sum(x => x.Total.SharedLibrary),
                SharedPersistence = results.Sum(x => x.Total.SharedPersistence)
            };

            // Count how many projects implemented an anti pattern
            var different = new AnalyzerResult
            {
                ApiVersioning = results.Count(x => x.Total.ApiVersioning > 0),
                CyclicDependency = results.Count(x => x.Total.CyclicDependency > 0),
                HardcodedEndpoints = results.Count(x => x.Total.HardcodedEndpoints > 0),
                Megaservice = results.Count(x => x.Total.Megaservice > 0),
                Monitoring = results.Count(x => x.Total.Monitoring > 0),
                SharedLibrary = results.Count(x => x.Total.SharedLibrary > 0),
                SharedPersistence = results.Count(x => x.Total.SharedPersistence > 0)

            };

            return (scaled, total, different);
        }
    }

    internal class Results
    {
        public Dictionary<string, decimal> RQ1_Weighted { get; set; }
        public Dictionary<string, decimal> RQ1_NotWeighted { get; set; }
        public AnalyzerResult RQ2_Scaled { get; set; }
        public AnalyzerResult RQ2_Total { get; set; }
        public AnalyzerResult RQ2_Different { get; set; }
        public IEnumerable<ProjectResult> ProjectResults { get; set; }
        public Report RawResults { get; set; }
    }
}
