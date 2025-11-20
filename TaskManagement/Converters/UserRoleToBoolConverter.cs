using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace TaskManagement.Converters;

public class UserRoleToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int)
        {
            return false;
        }
        int roleId =  (int)value;
        if (parameter is not string)
        {
            return false;
        }
        string name_element=  parameter as string;
        switch (name_element)
        {
            case "EMPL":
                return roleId == 1;
            case "ADMINMANAGER":
                return roleId > 1;
            case "ADMINEMPL":
                return roleId == 1 || roleId == 3;
            default:
                return false;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}