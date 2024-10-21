using System;
using System.Threading;
using System.Threading.Tasks;

namespace TextEditorLib.Managers
{
    public class AutoSaveManager
    {
        private CancellationTokenSource _cancellationTokenSource;
        private readonly TextEditor _textEditor;
        private readonly FileManager _fileManager;
        private readonly TimeSpan _interval;

        public AutoSaveManager(TextEditor textEditor, FileManager fileManager, TimeSpan interval)
        {
            _textEditor = textEditor;
            _fileManager = fileManager;
            _interval = interval;
        }

        public void StartAutoSave()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(_interval, _cancellationTokenSource.Token);

                        if (_textEditor.IsModified && !string.IsNullOrEmpty(_textEditor.FilePath))
                        {
                            await FileManager.SaveFileAsync(_textEditor.FilePath, _textEditor.TextContent);
                            _textEditor.IsModified = false;
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        // Автосохранение было остановлено
                    }
                }
            }, _cancellationTokenSource.Token);
        }

        public void StopAutoSave()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
