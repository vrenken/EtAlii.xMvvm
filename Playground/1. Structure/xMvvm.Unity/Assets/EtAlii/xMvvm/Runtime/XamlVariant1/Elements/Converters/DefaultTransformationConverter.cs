namespace EtAlii.xMvvm.XamlVariant1
{
    using System;
    using System.Globalization;

    public class DefaultTransformationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // var transform = (ViewModelTransform) value;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}