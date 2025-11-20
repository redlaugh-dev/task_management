using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TaskManagement.Converters;

public class DateOnlyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return null;
        }
        DateOnly date = (DateOnly)value;
        return new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        DateTimeOffset date = (DateTimeOffset)value;
        return DateOnly.FromDateTime(date.DateTime);
    }
}