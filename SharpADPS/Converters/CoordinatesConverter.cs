using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using CoreADPS.MailModels;

namespace WPFSharpADPS.Converters
{
    public class CoordinatesConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var coordinates = value as Coordinates;
            if (coordinates == null)
            {
                return null;
            }

            var latitudeLetter = coordinates.Latitude < 0 ? 'S' : 'N';
            var longitudeLetter = coordinates.Longitude < 0 ? 'W' : 'E';

            return String.Format("{0:0.00000}{1} {2:0.00000}{3}",
                                 Math.Abs(coordinates.Latitude), latitudeLetter, Math.Abs(coordinates.Longitude), longitudeLetter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
