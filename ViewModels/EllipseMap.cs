using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class EllipseMap : MapObject
    {
        double _EllipseWidth = default(double);
        public double Width { get { return _EllipseWidth; } set { Set(ref _EllipseWidth, value); } }

        double _EllipseHeight = default(double);
        public double Height { get { return _EllipseHeight; } set { Set(ref _EllipseHeight, value); } }

        //Geopoint _EllipseCenter = new Geopoint(new BasicGeoposition() { Latitude = 41.3825, Longitude = 2.176944, Altitude = 13 });
        //public Geopoint EllipseCenter { get { return _EllipseCenter; } set { Set(ref _EllipseCenter, value); } }

    }
}
