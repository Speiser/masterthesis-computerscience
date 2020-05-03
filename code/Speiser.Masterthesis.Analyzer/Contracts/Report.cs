using System.Collections.Generic;

namespace Speiser.Masterthesis.Analyzer.Contracts
{
    public class Report
    {
        public Dictionary<string, Finding[]> Findings { get; set; }
            = new Dictionary<string, Finding[]>();
    }
}
