using System.Windows;

namespace TextEditor.Windows
{
    public partial class FindReplaceWindow : Window
    {
        private readonly TextEditorLib.TextEditor _textEditor;
        private int _searchIndex = 0;

        public FindReplaceWindow(TextEditorLib.TextEditor editor)
        {
            InitializeComponent();
            _textEditor = editor;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = FindTextBox.Text;
            if (!string.IsNullOrEmpty(searchText) )
            {
                var mainWindow = Owner as MainWindow;
                var tabItem = mainWindow.TabControlEditors.SelectedItem as System.Windows.Controls.TabItem;
                var textBox = tabItem.Content as System.Windows.Controls.TextBox;
                textBox.Focus();
                textBox.Select(_searchIndex, searchText.Length);
                _searchIndex += searchText.Length;
            }
            else
            {
                MessageBox.Show("Текст не найден.", "Результат поиска", MessageBoxButton.OK, MessageBoxImage.Information);
                _searchIndex = 0;
            }
        }

        public void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = FindTextBox.Text;
            string replaceText = ReplaceTextBox.Text;
            if (! string.IsNullOrEmpty(searchText) )
            {
                _textEditor.Replace(searchText, replaceText);
                _searchIndex = 0;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
    }
}
