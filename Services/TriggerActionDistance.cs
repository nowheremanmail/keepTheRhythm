using System;
using System.Globalization;
using UniversalKeepTheRhythm.Others;
using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.Services
{
    public class TriggerActionDistance : _TriggerAction, IComparable<TriggerActionDistance>//, IComparer<TriggerActionDistance>
    {
        public double distance { get; set; }
        public Geocoordinate position { get; set; }

        public TriggerActionDistance(Engine eng) : base(eng) { }

        static public int Compare(TriggerActionDistance x, TriggerActionDistance y)
        {
            return x.distance.CompareTo(y.distance);
        }

        public int CompareTo(TriggerActionDistance other)
        {
            return distance.CompareTo(other.distance);
        }

        override public string toMessage(CultureInfo inf)
        {
            string _action = action;
            string tostring = "";

            if (action.Equals(Constants.LOOP_DETECTED))
            {
                double pace = (double)param;

                tostring = Utils.toPaceStringShort(pace, engine.unit, inf);
            }
            else if (action.Equals(Constants.DISTANCE_FROM) && param != null)
            {
                double tmp = (double)param;

                if (distance < (tmp / 2) || distance >= tmp)
                {
                    tostring = Utils.toReaderDistance(distance, engine.unit, engine.resourceLoader, inf);
                }
                else
                {
                    _action = Constants.DISTANCE_TO;
                    tmp = tmp - distance;
                    tostring = Utils.toReaderDistance(tmp, engine.unit, engine.resourceLoader, inf);
                }
            }
            else
            {
                tostring = Utils.toReaderDistance(distance, engine.unit, engine.resourceLoader, inf);
            }
            string msg = engine.resourceLoader.GetString(_action); //TODO, inf);
            if (msg != null && msg.Length > 0)
                msg = string.Format(msg, tostring, "{0}");
            return msg;
        }

    }

}


