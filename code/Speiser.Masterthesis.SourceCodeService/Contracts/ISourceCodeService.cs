using System.Collections.Generic;

namespace Speiser.Masterthesis.SourceCodeService.Contracts
{
    public interface ISourceCodeService
    {
        bool IsDotnetProject(string path);
        string GetProjectFile(string path);
        SourceProjectRepresentation GetParsedSource(string path);
        IEnumerable<ConfigurationFileInfo> GetConfigurationFiles(string path);
    }
}
