using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Services;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace UniversalKeepTheRhythm.Converters
{
    class TypeStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Sessions v = (Sessions)value;
            string name = parameter as string;

            if (name == "type")
            {
                // TODO
                string p = DataBaseManager.instance.GetPath(v.IdPath)?.Type;

                if (p == "R")
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/appbar.man.suitcase.run.png") };
                }
                else if (p == "W")
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/appbar.man.walk.png") };
                }
                else //if (lastSession != null && lastSession.Paths.type == "C")
                {
                    return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/appbar.bike.png") };
                }
            }
            else if (name == "description")
            {
                // TODO
                return DataBaseManager.instance.GetPath(v.IdPath)?.Description;
            }
            else if (name == "dayOfSession")
            {
                return Others.Utils.FromNOW(v.DayOfSession);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
