using System;
using System.Globalization;
using UniversalKeepTheRhythm.Others;

namespace UniversalKeepTheRhythm.Services
{
    public class TriggerActionNumber : _TriggerAction, IComparable<TriggerActionNumber>//, IComparer<TriggerActionTime>
    {
        public int num { get; set; }

        public TriggerActionNumber(Engine eng) : base(eng) { }

        static public int Compare(TriggerActionNumber x, TriggerActionNumber y)
        {
            int tmp = x.action.CompareTo(y.action);
            return tmp != 0 ? tmp : x.num.CompareTo(y.num);
        }

        public int CompareTo(TriggerActionNumber other)
        {
            int tmp = action.CompareTo(other.action);
            return tmp != 0 ? tmp : num.CompareTo(other.num);
        }

        override public string toMessage(CultureInfo inf)
        {
            string _action = action;
            string tostring = "";

            if (action.Equals(Constants.CHANGE_PACE))
            {
                _action = action + (question ? "Question" : "");

                if (param != null)
                {
                    tostring = Utils.toPaceStringShort((double)param, engine.unit, inf);
                }
            }
            else if (action.Equals(Constants.DO_PAUSE) || action.Equals(Constants.DO_CONTINUE))
            {
                _action = action + (question ? "Question" : "");

            }
            else
            {
                return num.ToString();
            }

            string msg = engine.resourceLoader.GetString(_action/* TODO , inf*/);
            if (msg != null && msg.Length > 0)
                msg = string.Format(msg, tostring, "{0}");
            return msg;
        }
    }

}


