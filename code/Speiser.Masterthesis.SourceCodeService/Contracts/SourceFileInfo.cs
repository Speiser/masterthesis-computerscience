using System.Diagnostics;

namespace Speiser.Masterthesis.SourceCodeService.Contracts
{
    [DebuggerDisplay("{Path}")]
    public class ConfigurationFileInfo
    {
        public string Path { get; set; }
        public string Content { get; set; }
    }
}
