using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UniversalKeepTheRhythm.Converters
{
    class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var _distance = (double)value;
                if (double.IsNaN(_distance)) return "";
                // TODO language to culture
                String unit = nowhereman.Properties.getProperty("units", "m");
                return Others.Utils.toDistanceString(_distance, unit, CultureInfo.CurrentCulture);
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                string txt = (string)value;

                String unit = nowhereman.Properties.getProperty("units", "m");

                return Others.Utils.fromDistanceString(txt, unit, CultureInfo.CurrentCulture);
            }

            throw new NotImplementedException();
        }
    }
}
