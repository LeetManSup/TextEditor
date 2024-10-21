using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TextEditorLib.Managers
{
    public class FileManager
    {
        public static async Task<string> OpenFileAsync(string filePath)
        {
            using StreamReader reader = new(filePath, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        public static async Task SaveFileAsync(string filePath, string content)
        {
            using StreamWriter writer = new(filePath, false, Encoding.UTF8);
            await writer.WriteAsync(content);
        }
    }
}
