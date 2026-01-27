using Microsoft.Maui.Controls;
using System;

namespace Consilium.Maui.Converters;
public class InverseBoolConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return value is bool b && !b;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return value; // not worried about converting back
    }
}