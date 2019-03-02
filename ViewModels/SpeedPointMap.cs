using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class SpeedPointMap : MapObject
    {
        double _Distance = default(double);
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }


        double _Speed = default(double);
        public double Speed { get { return _Speed; } set { Set(ref _Speed, value); } }

    }
}
