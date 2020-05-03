using System.Collections.Generic;
using System.Diagnostics;

namespace Speiser.Masterthesis.SourceCodeService.Contracts.Syntax
{
    [DebuggerDisplay("class {Identifier}")]
    public class CodeClass
    {
        public IEnumerable<CodeAttribute> Attributes { get; set; }
        public string Identifier { get; set; }
        public IEnumerable<string> Base { get; set; }
        public IEnumerable<CodeMethod> Methods { get; set; }
    }
}
