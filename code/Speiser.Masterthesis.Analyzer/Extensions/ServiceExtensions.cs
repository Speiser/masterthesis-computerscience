using System.Collections.Generic;
using System.Linq;
using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.Analyzer.Extensions
{
    internal static class ServiceExtensions
    {
        public static IEnumerable<Service> GetServicesWithSource(this IEnumerable<Service> services)
            => services.Where(s => !string.IsNullOrWhiteSpace(s.ProjectPath));

        public static IEnumerable<Service> GetDotnetProjects(
            this IEnumerable<Service> services,
            ISourceCodeService sourceCodeService
        ) => services.Where(s => sourceCodeService.IsDotnetProject(s.ProjectPath));

        public static bool IsAspNetProject(
            this Service service,
            ISourceCodeService sourceCodeService,
            IProjectFileReader projectFileReader)
        {
            var path = sourceCodeService.GetProjectFile(service.ProjectPath);
            var projectFile = projectFileReader.ParseProjectFile(path);
            return projectFile.Sdk == "Microsoft.NET.Sdk.Web";
        }
    }
}
