using System.Collections.Generic;

namespace Speiser.Masterthesis.Analyzer.Contracts
{
    public class Finding
    {
        public IEnumerable<string> Messages { get; set; }

        public string Analyzer { get; set; }
        public int ViolationCount { get; set; }
        public decimal RelativeViolationCount { get; set; }

        public decimal ScaledRelativeViolationCount { get; set; }
    }
}
