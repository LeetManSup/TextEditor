using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using TextEditorLib;
using System.Threading.Tasks;
using System.Windows.Data;
using TextEditor.Converters;
using TextEditor.Windows;
using System.Windows.Documents;

namespace TextEditor
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Горячие клавиши
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewFile_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, OpenFile_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveFile_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseTab_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Undo, Undo_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Redo, Redo_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Click));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Click));
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTab(null, "Новый документ");
        }

        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string content = await System.IO.File.ReadAllTextAsync(filePath);
                CreateNewTab(content, System.IO.Path.GetFileName(filePath), filePath);
            }
        }

        private void CreateNewTab(string content, string header, string filePath = null)
        {
            var textEditor = new TextEditorLib.TextEditor();
            if (content != null)
            {
                textEditor.TextContent = content;
                textEditor.FilePath = filePath;
            }

            var textBox = new TextBox
            {
                AcceptsReturn = true,
                AcceptsTab = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                DataContext = textEditor
            };

            textBox.SetBinding(TextBox.FontFamilyProperty, new Binding("SettingsManager.CurrentFontFamily"));
            textBox.SetBinding(TextBox.FontSizeProperty, new Binding("SettingsManager.CurrentFontSize"));
            textBox.SetBinding(TextBox.ForegroundProperty, new Binding("SettingsManager.CurrentTextColor") { Converter = Resources["ColorConverter"] as IValueConverter });
            textBox.SetBinding(TextBox.TextProperty, new Binding("TextContent") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            var tabItem = new TabItem
            {
                Header = header,
                Content = textBox,
                Tag = textEditor
            };

            TabControlEditors.Items.Add(tabItem);
            TabControlEditors.SelectedItem = tabItem;

            textEditor.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(TextEditorLib.TextEditor.IsModified))
                {
                    Dispatcher.Invoke(() =>
                    {
                        tabItem.Header = textEditor.IsModified ? $"{header}*" : header;
                    });
                }
            };
        }

        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                if (textEditor != null)
                {
                    if (!string.IsNullOrEmpty(textEditor.FilePath))
                    {
                        await textEditor.SaveFileAsync();
                        UpdateTabHeader(tabItem, textEditor.FilePath, textEditor.IsModified);
                    }
                    else
                    {
                        SaveAsFile_Click(sender, e);
                    }
                }
            }
        }

        private async void SaveAsFile_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                if (textEditor != null)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        await textEditor.SaveFileAsync(saveFileDialog.FileName);
                        UpdateTabHeader(tabItem, saveFileDialog.FileName, textEditor.IsModified);
                    }
                }
            }
        }

        private void UpdateTabHeader(TabItem tabItem, string filePath, bool isModified)
        {
            string header = System.IO.Path.GetFileName(filePath);
            tabItem.Header = isModified ? $"{header}*" : header;
        }

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                if (ConfirmSaveChanges(textEditor))
                {
                    textEditor.Dispose();
                    TabControlEditors.Items.Remove(tabItem);
                }
            }
        }

        private bool ConfirmSaveChanges(TextEditorLib.TextEditor textEditor)
        {
            if (textEditor.IsModified)
            {
                var result = MessageBox.Show($"Сохранить изменения в документе '{textEditor.FilePath ?? "Новый документ"}'?", "Подтверждение", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile_Click(null, null);
                    return true;
                }
                else if (result == MessageBoxResult.No)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            foreach (TabItem tabItem in TabControlEditors.Items)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                if (!ConfirmSaveChanges(textEditor))
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnClosing(e);
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                textEditor.Undo();
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                textEditor.Redo();
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textBox = tabItem.Content as TextBox;
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                textEditor.Copy(textBox.SelectedText);
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textBox = tabItem.Content as TextBox;
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                textEditor.Cut(textBox.SelectedText);
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textBox = tabItem.Content as TextBox;
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                int caretIndex = textBox.CaretIndex;
                textEditor.Paste(caretIndex);
                textBox.CaretIndex = caretIndex + (textEditor.ClipboardManager.Paste()?.Length ?? 0);
            }
        }

        private void FindReplace_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                var findReplaceWindow = new FindReplaceWindow(textEditor);
                findReplaceWindow.Owner = this;
                findReplaceWindow.Show();
            }
        }

        private void FontSettings_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                var fontSettingsWindow = new FontSettingsWindow(textEditor);
                fontSettingsWindow.Owner = this;
                fontSettingsWindow.ShowDialog();
            }
        }

        private void ColorSettings_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;
                var colorSettingsWindow = new ColorSettingsWindow(textEditor);
                colorSettingsWindow.Owner = this;
                colorSettingsWindow.ShowDialog();
            }
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            var tabItem = TabControlEditors.SelectedItem as TabItem;
            if (tabItem != null)
            {
                var textEditor = tabItem.Tag as TextEditorLib.TextEditor;

                try
                {
                    var previewWindow = new PrintPreviewWindow(
                        textEditor.TextContent,
                        textEditor.SettingsManager.CurrentFontFamily,
                        textEditor.SettingsManager.CurrentFontSize,
                        textEditor.SettingsManager.CurrentTextColor
                    );
                    previewWindow.Owner = this;

                    if (previewWindow.ShowDialog() == true)
                    {
                        PrintDialog printDialog = new PrintDialog();
                        if (printDialog.ShowDialog() == true)
                        {
                            printDialog.PrintDocument(((IDocumentPaginatorSource)previewWindow.DocumentViewer.Document).DocumentPaginator, "Печать документа");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при печати: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Текстовый редактор\nВерсия 1.0", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
