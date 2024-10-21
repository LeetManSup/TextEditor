using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TextEditor.Windows
{
    public partial class PrintPreviewWindow : Window
    {
        public PrintPreviewWindow(string content, string fontFamily, double fontSize, string textColor)
        {
            InitializeComponent();

            FlowDocument doc = new FlowDocument(new Paragraph(new Run(content)))
            {
                FontFamily = new FontFamily(fontFamily),
                FontSize = fontSize,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textColor)),
                PagePadding = new Thickness(50)
            };

            DocumentViewer.Document = doc;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;


        private void CancelButton_Click(object sender, RoutedEventArgs e) => DialogResult = false; 
    }
}
