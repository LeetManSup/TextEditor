using System.Collections.Generic;

namespace TextEditorLib.Managers
{
    public class UndoRedoManager
    {
        private readonly Stack<string> _undoStack;
        private readonly Stack<string> _redoStack;

        public UndoRedoManager()
        {
            _undoStack = new Stack<string>();
            _redoStack = new Stack<string>();
        }

        public void ExecuteAction(string currentState)
        {
            _undoStack.Push(currentState);
            _redoStack.Clear();
        }

        public string? Undo(string currentState)
        {
            if (_undoStack.Count > 0)
            {
                _redoStack.Push(currentState);
                return _undoStack.Pop();
            }
            return null;
        }

        public string? Redo(string currentState)
        {
            if (_redoStack.Count > 0)
            {
                _undoStack.Push(currentState);
                return _redoStack.Pop();
            }
            return null;
        }

        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
