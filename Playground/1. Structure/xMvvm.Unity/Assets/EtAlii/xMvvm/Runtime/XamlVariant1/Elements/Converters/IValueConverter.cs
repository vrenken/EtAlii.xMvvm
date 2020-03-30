namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.Globalization;

    public interface IValueConverter
    {
        object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}