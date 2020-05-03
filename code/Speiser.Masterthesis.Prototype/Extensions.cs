using System.Linq;
using Speiser.Masterthesis.Analyzer.Contracts;

namespace Speiser.Masterthesis.Prototype
{
    internal static class Extensions
    {
        public static ProjectResult TransformToProjectResult(this Finding[] findings, string project)
        {
            var apiVersioning = findings.Single(x => x.Analyzer == "ApiVersioning");
            var cyclicDependency = findings.Single(x => x.Analyzer == "CyclicDependency");
            var hardcodedEndpoints = findings.Single(x => x.Analyzer == "HardcodedEndpoints");
            var megaservice = findings.Single(x => x.Analyzer == "MegaService");
            var monitoring = findings.Single(x => x.Analyzer == "Monitoring");
            var sharedLibrary = findings.Single(x => x.Analyzer == "SharedLibrary");
            var sharedPersistence = findings.Single(x => x.Analyzer == "SharedPersistence");

            return new ProjectResult
            {
                Project = project,
                Relative = new AnalyzerResult
                {
                    ApiVersioning = apiVersioning.ScaledRelativeViolationCount,
                    CyclicDependency = cyclicDependency.ScaledRelativeViolationCount,
                    HardcodedEndpoints = hardcodedEndpoints.ScaledRelativeViolationCount,
                    Megaservice = megaservice.ScaledRelativeViolationCount,
                    Monitoring = monitoring.ScaledRelativeViolationCount,
                    SharedLibrary = sharedLibrary.ScaledRelativeViolationCount,
                    SharedPersistence = sharedPersistence.ScaledRelativeViolationCount
                },
                Total = new AnalyzerResult
                {
                    ApiVersioning = apiVersioning.ViolationCount,
                    CyclicDependency = cyclicDependency.ViolationCount,
                    HardcodedEndpoints = hardcodedEndpoints.ViolationCount,
                    Megaservice = megaservice.ViolationCount,
                    Monitoring = monitoring.ViolationCount,
                    SharedLibrary = sharedLibrary.ViolationCount,
                    SharedPersistence = sharedPersistence.ViolationCount
                }
            };
        }
    }
}
