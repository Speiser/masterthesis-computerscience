using System.Collections.Generic;

namespace Speiser.Masterthesis.SourceCodeService.Contracts
{
    public class ProjectFile
    {
        public string Sdk { get; set; }
        public string FilePath { get; set; }
        public IEnumerable<string> Libraries { get; set; }
    }
}
