using System.Collections.Generic;
using Speiser.Masterthesis.SourceCodeService.Contracts;

namespace Speiser.Masterthesis.SourceCodeService.Internals
{
    internal class CachedSourceCodeService : ISourceCodeService
    {
        private readonly Dictionary<string, string> cachedProjectFile = new Dictionary<string, string>();
        private readonly Dictionary<string, SourceProjectRepresentation> cachedSource = new Dictionary<string, SourceProjectRepresentation>();
        private readonly Dictionary<string, IEnumerable<ConfigurationFileInfo>> cachedFileContents = new Dictionary<string, IEnumerable<ConfigurationFileInfo>>();
        private readonly Dictionary<string, bool> cachedIsDotnetProject = new Dictionary<string, bool>();
        private readonly ISourceCodeService sourceCodeService;

        public CachedSourceCodeService(ISourceCodeService sourceCodeService)
        {
            this.sourceCodeService = sourceCodeService;
        }

        public bool IsDotnetProject(string path)
        {
            if (this.cachedIsDotnetProject.TryGetValue(path, out var result))
                return result;

            result = this.sourceCodeService.IsDotnetProject(path);
            this.cachedIsDotnetProject.Add(path, result);
            return result;
        }

        public string GetProjectFile(string path)
        {
            if (this.cachedProjectFile.TryGetValue(path, out var projectFile))
                return projectFile;

            projectFile = this.sourceCodeService.GetProjectFile(path);
            this.cachedProjectFile.Add(path, projectFile);
            return projectFile;
        }

        public SourceProjectRepresentation GetParsedSource(string path)
        {
            if (this.cachedSource.TryGetValue(path, out var source))
                return source;

            source = this.sourceCodeService.GetParsedSource(path);
            this.cachedSource.Add(path, source);
            return source;
        }

        public IEnumerable<ConfigurationFileInfo> GetConfigurationFiles(string path)
        {
            if (this.cachedFileContents.TryGetValue(path, out var contents))
                return contents;

            contents = this.sourceCodeService.GetConfigurationFiles(path);
            this.cachedFileContents.Add(path, contents);
            return contents;
        }
    }
}
