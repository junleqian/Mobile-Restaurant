namespace Gour.Phone.Converters
{
    using System;
    using System.Windows.Data;

    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var date = (DateTime)value;

                return date.ToShortDateString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                return DateTime.Parse(value.ToString());
            }
            catch
            {
                return null;
            }
        }
    }
}
