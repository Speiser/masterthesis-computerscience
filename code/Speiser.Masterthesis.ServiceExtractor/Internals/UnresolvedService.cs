using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Speiser.Masterthesis.ServiceExtractor.Internals
{
    internal class UnresolvedService
    {
        public string Name { get; set; }
        public IEnumerable<string> DependsOn { get; set; }
        public string ProjectPath { get; set; }
    }
}
