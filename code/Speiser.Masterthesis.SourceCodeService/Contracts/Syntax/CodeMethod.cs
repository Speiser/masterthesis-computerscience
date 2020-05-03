using System.Collections.Generic;
using System.Diagnostics;

namespace Speiser.Masterthesis.SourceCodeService.Contracts.Syntax
{
    [DebuggerDisplay("{Identifier}()")]
    public class CodeMethod
    {
        public string Identifier { get; set; }
        public IEnumerable<CodeAttribute> Attributes { get; set; }
        public IEnumerable<CodeStatement> Statements { get; set; }
    }
}
