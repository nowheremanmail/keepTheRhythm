using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class PacePointMap : MapObject
    {
        double _Distance = default(double);
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }


        double _Pace = default(double);
        public double Pace { get { return _Pace; } set { Set(ref _Pace, value); } }
    }
}
