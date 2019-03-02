using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Template10.Mvvm;
using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.ViewModels
{

    public class ShowInfoCanvas : ObjectCanvas
    {

        Geopoint _Location = default(Geopoint);
        public Geopoint Location { get { return _Location; } set { Set(ref _Location, value); } }

        // for interaction
        double _pacePoint = default(double);
        public double pacePoint { get { return _pacePoint; } set { Set(ref _pacePoint, value); } }

        double _speedPoint = default(double);
        public double speedPoint { get { return _speedPoint; } set { Set(ref _speedPoint, value); } }
        double _altitudePoint = default(double);
        public double altitudePoint { get { return _altitudePoint; } set { Set(ref _altitudePoint, value); } }

        double _point = default(double);
        public double point { get { return _point; } set { Set(ref _point, value); } }

        TimeSpan _time = default(TimeSpan);
        public TimeSpan time { get { return _time; } set { Set(ref _time, value); } }


    }

}
