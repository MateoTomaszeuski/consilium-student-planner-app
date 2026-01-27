using Microsoft.Maui.Controls;
using System;

namespace Consilium.Maui.Converters;
public class SeeMoreSeeLessConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return (bool)value ? "See Less" : "See Details"; // Returns "See Less" if expanded, else "See Details"
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
        return null; // We don't need to convert back in this case
    }
}