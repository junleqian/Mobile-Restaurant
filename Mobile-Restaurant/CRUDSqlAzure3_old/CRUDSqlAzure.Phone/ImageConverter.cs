using System;
using System.Windows.Data;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Phone;

namespace Microsoft.Samples.CRUDSqlAzure.Phone
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            /*
            byte[] buffer = value as byte[];
            Stream memStream = new MemoryStream(buffer);
            WriteableBitmap wbimg = PictureDecoder.DecodeJpeg(memStream);
            return wbimg;
            */
            if((byte[])value == null)
            {
                BitmapImage defaultImage = new BitmapImage(new System.Uri("..\\Images\\gour_noimage.png", UriKind.RelativeOrAbsolute));
                return defaultImage;
            }

            Stream memStream = new MemoryStream((byte[])value);
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