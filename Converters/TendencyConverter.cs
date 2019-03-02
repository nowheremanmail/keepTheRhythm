using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace UniversalKeepTheRhythm.Converters
{
    class TendencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var Tendency = (int)value;
                if (Tendency > 0)
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/down.png") };
                }
                else if (Tendency < 0)
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/up.png") };
                }
                else
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/equal.png") };
                }
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
