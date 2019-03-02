using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class PolylineMap: MapObject
    {
        public Geopath MapRoute { get; set; }
        public int StrokeThickness { get; set; }
            public Color StrokeColor { get; set; }
    }
}
