using System.ComponentModel;
using System.Data;

namespace TextEditorLib.Managers
{
    public class SettingsManager : INotifyPropertyChanged
    {
        #region Fields

        private string _currentFontFamily = "Segoe UI";
        private double _currentFontSize = 14;
        private string _currentTextColor = "#000000";

        public string CurrentFontFamily
        {
            get => _currentFontFamily;
            private set
            {
                if (_currentFontFamily != value)
                {
                    _currentFontFamily = value;
                    OnPropertyChanged(nameof(CurrentFontFamily));
                }
            }
        }

        public double CurrentFontSize
        {
            get => _currentFontSize;
            private set
            {
                if (_currentFontSize != value)
                {
                    _currentFontSize = value;
                    OnPropertyChanged(nameof(CurrentFontSize));
                }
            }
        }

        public string CurrentTextColor
        {
            get => _currentTextColor;
            private set
            {
                if (_currentTextColor != value)
                {
                    _currentTextColor = value;
                    OnPropertyChanged(nameof(CurrentTextColor));
                }
            }
        }

        #endregion


        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion


        #region Methods

        public void UpdateFont(string fontFamily, double fontSize)
        {
            CurrentFontFamily = fontFamily;
            CurrentFontSize = fontSize;
        }

        public void UpdateTextColor(string color) => CurrentTextColor = color;

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
