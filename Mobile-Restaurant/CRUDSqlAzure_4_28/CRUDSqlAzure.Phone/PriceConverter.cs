using System;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone;

namespace Microsoft.Samples.CRUDSqlAzure.Phone
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string priceString = "";
            if ((decimal?)value != null)
            {
                decimal myPrice = ((decimal?)value).Value;
                priceString = myPrice.ToString("C");
            }
            return priceString;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}