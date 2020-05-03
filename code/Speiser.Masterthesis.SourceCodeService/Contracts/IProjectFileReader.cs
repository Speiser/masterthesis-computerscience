namespace Speiser.Masterthesis.SourceCodeService.Contracts
{
    public interface IProjectFileReader
    {
        ProjectFile ParseProjectFile(string path);
    }
}
