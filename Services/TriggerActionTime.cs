using System;
using System.Globalization;
using UniversalKeepTheRhythm.Others;

namespace UniversalKeepTheRhythm.Services
{
    public class TriggerActionTime : _TriggerAction, IComparable<TriggerActionTime>//, IComparer<TriggerActionTime>
    {
        public TimeSpan time { get; set; }

        public TriggerActionTime(Engine eng) : base(eng) { }
        static public int Compare(TriggerActionTime x, TriggerActionTime y)
        {
            return x.time.CompareTo(y.time);
        }

        public int CompareTo(TriggerActionTime other)
        {
            return time.CompareTo(other.time);
        }

        override public string toMessage(CultureInfo inf)
        {
            string _action = action;
            string tostring = "";

            if (action.Equals(Constants.MINUTES_FROM) && param != null)
            {
                TimeSpan tmp = (TimeSpan)param;

                if (time.Ticks < (tmp.Ticks / 2) || time.Ticks >= tmp.Ticks)
                {
                    tostring = Utils.toReaderTime(time, engine.resourceLoader, inf);
                }
                else
                {
                    _action = Constants.MINUTES_TO;
                    tmp = tmp.Subtract(time);

                    tostring = Utils.toReaderTime(tmp, engine.resourceLoader, inf);
                }
            }
            else
            {
                tostring = Utils.toReaderTime(time, engine.resourceLoader, inf);
            }

            string msg = engine.resourceLoader.GetString(_action); // TODO, inf);
            if (msg != null && msg.Length > 0)
                msg = string.Format(msg, tostring, "{0}");
            return msg;
        }
    }

}


