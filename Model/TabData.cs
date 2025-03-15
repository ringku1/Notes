using System;
using System.ComponentModel;

namespace WinUIApp1.Model;
public class TabData : INotifyPropertyChanged {
    private Tuple<int, int> _cursorPosition = new(1, 1);
    private int _characterCount = 0;
    private int _ln = 1;
    private int _col = 1;

    public Tuple<int, int> CursorPosition {
        get => _cursorPosition;
        set {
            if (!_cursorPosition.Equals(value)) {
                _cursorPosition = value;
                LineNumber = value.Item1; // Update LineNumber
                ColumnNumber = value.Item2; // Update ColumnNumber
                OnPropertyChanged(nameof(CursorPosition));
            }
        }
    }

    public int CharacterCount {
        get => _characterCount;
        set {
            if (_characterCount != value) {
                _characterCount = value;
                OnPropertyChanged(nameof(CharacterCount));
            }
        }
    }

    public int LineNumber {
        get => _ln;
        set {
            if (_ln != value) {
                _ln = value;
                OnPropertyChanged(nameof(LineNumber));
            }
        }
    }

    public int ColumnNumber {
        get => _col;
        set {
            if (_col != value) {
                _col = value;
                OnPropertyChanged(nameof(ColumnNumber));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}