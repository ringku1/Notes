using Microsoft.UI.Xaml.Data;
using System;

namespace WinUIApp1.Converter;
public class CharacterCountConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
        if (value is int count) {
            return $"{count} characters"; // Format it as "X characters"
        }
        return "0 characters"; // Default case
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
        throw new NotImplementedException();
    }
}