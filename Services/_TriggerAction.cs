using System.Globalization;

namespace UniversalKeepTheRhythm.Services
{
    abstract public class _TriggerAction
    {
        protected Engine engine;

        public _TriggerAction(Engine eng)
        {
            this.engine = eng;
        }
        public string title { get; set; }
        public string description { get; set; }

        public string action { get; set; }
        public object param { get; set; }

        public bool question { get; set; }
        public string cancels { get; set; }

        abstract public string toMessage(CultureInfo inf);
    }

}


