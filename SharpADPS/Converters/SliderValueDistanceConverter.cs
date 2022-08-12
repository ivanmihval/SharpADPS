using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFSharpADPS.Converters
{
    class SliderValueDistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double))
            {
                return "Undefined";
            }

            var metersValue = (double) value;
            if (metersValue < 2000)
            {
                return String.Format("{0} m", (int) Math.Round(metersValue));
            }
            if (metersValue < 50000)
            {
                return String.Format("{0:0.0} km", metersValue / 1000.0);
            }

            return String.Format("{0} km", (int) Math.Round(metersValue / 1000.0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
