using System;
using System.ComponentModel;
using System.Threading.Tasks;
using TextEditorLib.Managers;

namespace TextEditorLib
{
    public class TextEditor : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private string _textContent;
        private bool _isModified;
        private string _filePath;

        public string TextContent
        {
            get => _textContent;
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(TextContent));
                    UndoRedoManager.ExecuteAction(_textContent);
                }
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    OnPropertyChanged(nameof(IsModified));
                }
            }
        }

        public AutoSaveManager AutoSaveManager { get; }
        public ClipboardManager ClipboardManager { get; }
        public FileManager FileManager { get; }
        public SearchReplaceManager SearchReplaceManager { get; }
        public SettingsManager SettingsManager { get; }
        public UndoRedoManager UndoRedoManager { get; }

        #endregion


        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion


        #region Constructors

        public TextEditor()
        {
            AutoSaveManager = new AutoSaveManager(this, FileManager, TimeSpan.FromMinutes(1));
            ClipboardManager = new ClipboardManager();
            FileManager = new FileManager();
            SearchReplaceManager = new SearchReplaceManager();
            SettingsManager = new SettingsManager();
            UndoRedoManager = new UndoRedoManager();

            AutoSaveManager.StartAutoSave();
        }

        #endregion


        #region Methods

        public async Task OpenFileAsync(string path)
        {
            string content = await FileManager.OpenFileAsync(path);
            TextContent = content;
            FilePath = path;
            IsModified = false;
            UndoRedoManager.ClearHistory();
        }

        public async Task SaveFileAsync(string? path = null)
        {
            if (path != null)
            {
                FilePath = path;
            }

            if (string.IsNullOrEmpty(FilePath))
            {
                throw new InvalidOperationException("Путь к файлу не указан");
            }

            await FileManager.SaveFileAsync(FilePath, TextContent);
            IsModified = false;
        }

        public void Undo()
        {
            string previousState = UndoRedoManager.Undo(TextContent);
            if (previousState != null)
            {
                _textContent = previousState;
                OnPropertyChanged(nameof(TextContent));
            }
        }

        public void Redo()
        {
            string nextState = UndoRedoManager.Redo(TextContent);
            if (nextState != null)
            {
                _textContent = nextState;
                OnPropertyChanged(nameof(TextContent));
            }
        }

        public int Find(string searchText, int startIndex = 0) => SearchReplaceManager.FindText(TextContent, searchText, startIndex);

        public void Replace(string searchText, string replaceText) => TextContent = SearchReplaceManager.ReplaceText(TextContent, searchText, replaceText);

        public void Copy(string selectedText) => ClipboardManager.Copy(selectedText);

        public void Cut(string selectedText)
        {
            ClipboardManager.Copy(selectedText);
            TextContent = TextContent.Replace(selectedText, string.Empty);
        }

        public void Paste(int insertionIndex)
        {
            string clipboardText = ClipboardManager.Paste();
            if (!string.IsNullOrEmpty(clipboardText))
            {
                TextContent = TextContent.Insert(insertionIndex, clipboardText);
            }
        }

        public void UpdateFont(string fontFamily, double fontSize) => SettingsManager.UpdateFont(fontFamily, fontSize);

        public void UpdateTextColor(string color) => SettingsManager.UpdateTextColor(color);

        public void Dispose() => AutoSaveManager.StopAutoSave();

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
