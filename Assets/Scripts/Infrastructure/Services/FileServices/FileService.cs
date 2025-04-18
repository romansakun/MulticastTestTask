using System.IO;

namespace Infrastructure.Services
{
    public class FileService : IFileService
    {
        public bool TryReadAllText(string path, out string content)
        {
            if (File.Exists(path) == false)
            {
                content = null;
                return false;
            }
            content = File.ReadAllText(path);
            return true;
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }
    }
}