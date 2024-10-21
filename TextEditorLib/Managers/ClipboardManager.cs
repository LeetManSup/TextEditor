using System.Windows;
using System.Windows.Forms;

namespace TextEditorLib.Managers
{
    public class ClipboardManager
    {
        public void Copy(string text) => Clipboard.SetText(text);

        public string Paste()
        {
            if (Clipboard.ContainsText())
                return Clipboard.GetText();

            return string.Empty;
        }
    }
}
