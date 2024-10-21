using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using TextEditorLib;

namespace TextEditor.Windows
{
    public partial class ColorSettingsWindow : Window
    {
        private readonly TextEditorLib.TextEditor _textEditor;

        public ColorSettingsWindow(TextEditorLib.TextEditor editor)
        {
            InitializeComponent();
            _textEditor = editor;

            var colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);

            ColorComboBox.ItemsSource = colorProperties.Select(p => p.Name);

            var currentColor = (Color)ColorConverter.ConvertFromString(_textEditor.SettingsManager.CurrentTextColor);
            var currentColorName = colorProperties.FirstOrDefault(p => ((Color)p.GetValue(null)).Equals(currentColor))?.Name;
            ColorComboBox.SelectedItem = currentColorName;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (ColorComboBox.SelectedItem != null)
            {
                string selectedColorName = ColorComboBox.SelectedItem.ToString();
                var color = (Color)ColorConverter.ConvertFromString(selectedColorName);
                var colorString = color.ToString();

                _textEditor.UpdateTextColor(colorString);
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите цвет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
