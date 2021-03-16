namespace ProgramB.Services
{
    public interface IHandFromFileService
    {
        (string Id, string Hand) GetHandFromFileA(string filePath);
        (string Id, string Hand) GetHandFromFileB(string filePath);
        void ResetSource();
    }
}