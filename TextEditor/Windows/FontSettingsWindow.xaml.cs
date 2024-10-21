using System.Linq;
using System.Windows;
using System.Windows.Media;
using TextEditorLib;

namespace TextEditor.Windows
{
    public partial class FontSettingsWindow : Window
    {
        private readonly TextEditorLib.TextEditor _textEditor;

        public FontSettingsWindow(TextEditorLib.TextEditor editor)
        {
            InitializeComponent();
            _textEditor = editor;

            FontFamilyComboBox.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.SelectedItem = new FontFamily(_textEditor.SettingsManager.CurrentFontFamily);
            FontSizeTextBox.Text = _textEditor.SettingsManager.CurrentFontSize.ToString();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFont = FontFamilyComboBox.SelectedItem as FontFamily;
            if (double.TryParse(FontSizeTextBox.Text, out double fontSize))
            {
                _textEditor.UpdateFont(selectedFont.Source, fontSize);
                Close();
            }
            else
            {
                MessageBox.Show("Введите корректный размер шрифта.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
