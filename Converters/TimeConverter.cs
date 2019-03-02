using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UniversalKeepTheRhythm.Converters
{
    class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return "";

            if (value is Int64)
            {
                var time = TimeSpan.FromTicks((Int64)value);
                return Others.Utils.toTimeStringF(time);
            }
            else if (value is long)
            {
                var time = TimeSpan.FromTicks((long)value);
                return Others.Utils.toTimeStringF(time);
            }
            else if (value is TimeSpan)
            {
                var time = (TimeSpan)value;
                if (time != TimeSpan.MinValue)
                    return Others.Utils.toTimeStringF(time);
                else
                    return "";
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;

            if (value is string)
            {
                TimeSpan result;

                if (TimeSpan.TryParse((string)value, out result))// TODO culture
                {
                    if (targetType == typeof(TimeSpan))
                    {
                        return result;
                    }
                    else if (targetType == typeof(Int64) || targetType == typeof(long))
                    {
                        return result.Ticks;
                    }
                }
            }
            throw new NotImplementedException();
        }
    }
}
