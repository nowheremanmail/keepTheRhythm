using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalKeepTheRhythm.model;
using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class EnrichedMesure 
    {
        public EnrichedMesure(Mesures a)
        {
            Speed = a.Speed.HasValue ? a.Speed.Value : double.NaN;
            Position= new BasicGeoposition() { Latitude = a.Latitude, Longitude = a.Longitude, Altitude = a.Speed.HasValue ? a.Speed.Value : 0.0};
            Id = a.Id;
            Altitude = Altitude = a.Altitude.HasValue ? a.Altitude.Value : double.NaN;
        }
        public double Altitude { get; set; }
        public long Id { get; set; }
        public BasicGeoposition Position {get; set;}
        //public double Pace { get; set; }
        public double Distance { get; set; }
        public long Time { get; set; }
        public double Rhythm { get; set; }
        public double Speed { get; set; }
    }
}
