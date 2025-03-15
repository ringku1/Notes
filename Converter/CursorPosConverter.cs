using Microsoft.UI.Xaml.Data;
using System;
using System.Diagnostics;

namespace WinUIApp1.Converter;

class CursorPosConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) {
        if (value is Tuple<int, int> cursorPosition) {
            return $"Ln {cursorPosition.Item1}, Col {cursorPosition.Item2}";
        }
        return "Ln 1, Col 1";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
        throw new NotImplementedException();
    }
}
