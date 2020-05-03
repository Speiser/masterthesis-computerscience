using System.Collections.Generic;
using System.Diagnostics;
using Speiser.Masterthesis.SourceCodeService.Contracts.Syntax;

namespace Speiser.Masterthesis.SourceCodeService.Contracts
{
    [DebuggerDisplay("{Information}")]
    public class SourceProjectRepresentation
    {
        public string ProjectFile { get; set; }
        public IEnumerable<CodeClass> Types { get; set; }
    }
}
