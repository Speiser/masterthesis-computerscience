using System.Diagnostics;

namespace Speiser.Masterthesis.SourceCodeService.Contracts.Syntax
{
    [DebuggerDisplay("{Statement}")]
    public class CodeStatement
    {
        public string Statement { get; set; }
    }
}
