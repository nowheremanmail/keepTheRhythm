using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class LapDetail
    {
        public BasicGeoposition Position { get; set; }
        public string Title { get; set; }

        public long Time { get; set; }
        public double Distance { get; set; }
        public double Pace { get; set; }
        public int Tendency { get; set; }
    }

}


