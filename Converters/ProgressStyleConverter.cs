using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace UniversalKeepTheRhythm.Converters
{
    class ProgressStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //System.Diagnostics.Debug.WriteLine("PASA " + parameter);

            ScreenStatus v = (ScreenStatus)value;
            string p = parameter.ToString();

            if (targetType == typeof(Style))
            {
                if (v._modeDay)
                {
                    return Application.Current.Resources[p];
                }
                else
                {
                    return Application.Current.Resources[p + "L"];
                }
            }
            else
            {
                if (p.Equals("appbar.sync.rest.png"))
                {
                    if (v._rotation)
                    {
                        if (v._modeDay)
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/Lappbar.sync.rest.on.png")};
                        else
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/appbar.sync.rest.on.png")};
                    }
                    else
                    {
                        if (v._modeDay)
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/Lappbar.sync.rest.png")};
                        else
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/appbar.sync.rest.png")};
                    }
                }
                else if (p.Equals("photo.light.off.png"))
                {
                    if (v._modeDay)
                        return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/Lappbar.moon.waning.crescent.png")};
                    else
                        return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/appbar.weather.sun.png")};
                }
                else if (p.Equals("map.direction.png"))
                {
                    if (v._followCompass)
                    {
                        if (v._modeDay)
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/Lmap.direction.off.png")};
                        else
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/map.direction.off.png")};
                    }
                    else
                    {
                        if (v._modeDay)
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/Lmap.direction.png")};
                        else
                            return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/map.direction.png")};
                    }
                }
                else
                {
                    if (v._modeDay)
                        return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/L" + p) };
                    else
                        return new BitmapImage() { UriSource = new Uri("ms-appx:///Images/" + p) };
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
