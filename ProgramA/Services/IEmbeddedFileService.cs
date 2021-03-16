namespace ProgramA.Services
{
    public interface IEmbeddedFileService
    {
        string GetHandFromFileA();
        string GetHandFromFileB();
        void ResetSource();
    }
}