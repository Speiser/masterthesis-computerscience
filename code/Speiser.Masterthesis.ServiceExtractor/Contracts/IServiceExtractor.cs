using System.Collections.Generic;

namespace Speiser.Masterthesis.ServiceExtractor.Contracts
{
    public interface IServiceExtractor
    {
        bool IsUsedByProject(string projectPath);
        IEnumerable<Service> GetServices(string projectPath);
    }
}
