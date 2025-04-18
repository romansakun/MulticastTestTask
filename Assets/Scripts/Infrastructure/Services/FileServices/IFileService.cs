namespace Infrastructure.Services
{
    public interface IFileService
    {
        bool TryReadAllText(string path, out string content);
        void WriteAllText(string path, string content);
    }
}