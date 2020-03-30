namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.Globalization;

    public class LinearTransformationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}