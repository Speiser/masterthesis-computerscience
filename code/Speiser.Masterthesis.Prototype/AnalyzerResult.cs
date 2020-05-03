namespace Speiser.Masterthesis.Prototype
{
    public class AnalyzerResult
    {
        public decimal ApiVersioning { get; set; }
        public decimal CyclicDependency { get; set; }
        public decimal HardcodedEndpoints { get; set; }
        public decimal Megaservice { get; set; }
        public decimal Monitoring { get; set; }
        public decimal SharedLibrary { get; set; }
        public decimal SharedPersistence { get; set; }
    }
}
