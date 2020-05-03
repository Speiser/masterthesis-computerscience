using Speiser.Masterthesis.ServiceExtractor.Contracts;
using Speiser.Masterthesis.ServiceExtractor.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Speiser.Masterthesis.ServiceExtractor.Internals
{
    internal class DockerComposeServiceExtractor : IServiceExtractor
    {
        public IEnumerable<Service> GetServices(string projectPath)
        {
            var stream = this.CreateYamlStream(this.GetFilePath(projectPath));
            var unresolved = this.ParseUnresolvedServices(stream);
            return this.ResolveServices(unresolved, projectPath);
        }

        public bool IsUsedByProject(string projectPath) => File.Exists(this.GetFilePath(projectPath));

        private string GetFilePath(string projectPath) => $"{projectPath}/docker-compose.yml";

        private YamlStream CreateYamlStream(string file)
        {
            using var fs = new FileStream(file, FileMode.Open);
            using var reader = new StreamReader(fs);

            var yaml = new YamlStream();
            yaml.Load(reader);
            return yaml;
        }

        private IEnumerable<UnresolvedService> ParseUnresolvedServices(YamlStream yaml)
        {
            var root = yaml.Documents.First().RootNode as YamlMappingNode;
            var serviceNode = root.GetChild("services") as YamlMappingNode;

            var result = new List<UnresolvedService>();
            foreach (var child in serviceNode.Children)
            {
                var service = new UnresolvedService { Name = child.Key.ToString() };
                var serviceMappingNode = child.Value as YamlMappingNode;
                service.DependsOn = serviceMappingNode.GetChild("depends_on") is YamlSequenceNode dependsOn
                    ? dependsOn.Children.Select(c => c.ToString()).ToArray()
                    : Enumerable.Empty<string>();

                var buildInfo = serviceMappingNode.GetChild("build");
                service.ProjectPath = ExtractProjectPath(buildInfo);
                result.Add(service);
            }

            return result;
        }

        private static string ExtractProjectPath(YamlNode buildInfo)
        {
            if (buildInfo is YamlMappingNode buildMappingNode)
            {
                var projectPathBuilder = new StringBuilder();
                var context = buildMappingNode.GetChild("context");
                if (context != null)
                {
                    // Note: Does not work for ../ paths!
                    projectPathBuilder.Append(context.ToString().Trim('.'));
                }
                return projectPathBuilder.Append(buildMappingNode.GetChild("dockerfile").ToString()).ToString();
            }
            else if (buildInfo is YamlScalarNode buildScalarNode)
            {
                return buildScalarNode.ToString();
            }

            return null;
        }

        private IEnumerable<Service> ResolveServices(
            IEnumerable<UnresolvedService> unresolvedServices,
            string projectPath)
        {
            var dict = unresolvedServices.ToDictionary(us => us.Name, us => new Service { Name = us.Name });
            foreach (var us in unresolvedServices)
            {
                var service = dict[us.Name];
                service.DependsOn = us.DependsOn.Select(d =>
                {
                    if (!dict.TryGetValue(d, out var service))
                    {
                        throw new ArgumentException($"No service with the name {d} found. Referenced by {us.Name}.");
                    }
                    return service;
                });
                if (!string.IsNullOrWhiteSpace(us.ProjectPath))
                {
                    service.ProjectPath = BuildProjectPath(projectPath, us.ProjectPath);
                }
            }
            return dict.Values;
        }

        private static string BuildProjectPath(string basePath, string path)
            => $"{basePath}\\{path.Replace("/", "\\").Replace("Dockerfile", string.Empty)}";
    }
}
