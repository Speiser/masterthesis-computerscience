using System.Collections.Generic;
using System.Diagnostics;

namespace Speiser.Masterthesis.SourceCodeService.Contracts.Syntax
{
    [DebuggerDisplay("[{Identifier}]")]
    public class CodeAttribute
    {
        public string Identifier { get; set; }
        public IEnumerable<string> Param { get; set; }
    }
}
