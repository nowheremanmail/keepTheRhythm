using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.Services
{
    public class TriggerAction
    {
        public Geocoordinate geo { get; set; }
        public int direction { get; set; }                          // 0 -> on, 1 -> going to, 2 -> leaving from
        public double units { get; set; }

        public string action { get; set; }
    }

}


