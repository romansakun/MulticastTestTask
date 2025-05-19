using UnityEngine;

namespace Infrastructure.Services
{
    public class PlayerPrefsService : IFileService
    {
        public bool TryReadAllText(string path, out string content)
        {
            var text = PlayerPrefs.GetString(path, null);
            if (text == null)
            {
                content = null;
                return false;
            }
            content = text;
            return true;
        }

        public void WriteAllText(string path, string content)
        {
            PlayerPrefs.SetString(path, content);
        }
    }
}