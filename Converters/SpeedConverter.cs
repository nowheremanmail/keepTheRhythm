using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UniversalKeepTheRhythm.Converters
{
    class SpeedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var _distance = (double)value;
                if (double.IsNaN(_distance)) return "";
                // TODO language to culture
                String unit = nowhereman.Properties.getProperty("units", "m");
                return Others.Utils.toKmhString(_distance, unit, System.Globalization.CultureInfo.CurrentCulture);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
