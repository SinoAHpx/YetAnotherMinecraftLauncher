using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace YetAnotherMinecraftLauncher.Models.Converters;

internal class ZeroToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i) return i == 0;

        return new BindingNotification(new InvalidCastException(),
            BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal class ZeroToInvisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i) return i != 0;

        return new BindingNotification(new InvalidCastException(),
            BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}