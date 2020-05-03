using System.Collections.Generic;

namespace Speiser.Masterthesis.ServiceExtractor.Contracts
{
    public class Service
    {
        public string Name { get; set; }
        public IEnumerable<Service> DependsOn { get; set; }
        public string ProjectPath { get; set; }
    }
}
