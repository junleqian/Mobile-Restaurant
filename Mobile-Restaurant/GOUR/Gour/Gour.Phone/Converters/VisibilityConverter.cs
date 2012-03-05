namespace Gour.Phone.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            bool visibile = false;
            bool reverse = false;

            bool.TryParse(value.ToString(), out visibile);
            if (parameter != null)
            {
                bool.TryParse(parameter.ToString(), out reverse);
            }

            return !reverse
                    ? visibile ? Visibility.Visible : Visibility.Collapsed
                    : visibile ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility;

            try
            {
                visibility = (Visibility)value;
            }
            catch
            {
                visibility = Visibility.Collapsed;
            }

            return visibility == Visibility.Visible ? true : false;
        }
    }
}
