using System;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;

namespace Microsoft.Samples.CRUDSqlAzure.Phone
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MemoryStream memStream = new MemoryStream((byte[])value);
            memStream.Seek(0, SeekOrigin.Begin);
            BitmapImage empImage = new BitmapImage();
            empImage.SetSource(memStream);
            return empImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}