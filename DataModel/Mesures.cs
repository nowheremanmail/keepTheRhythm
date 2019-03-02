namespace UniversalKeepTheRhythm.model
{
    public partial class Mesures
    {
        private int _idSession;

        private long _id;

        private System.Nullable<double> _speed;

        private System.Nullable<double> _altitude;

        private double _longitude;

        private double _latitude;

        private string _action;

        public int IdSession
        {
            get
            {
                return _idSession;
            }

            set
            {
                _idSession = value;
            }
        }

        public long Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public double? Speed
        {
            get
            {
                return _speed;
            }

            set
            {
                _speed = value;
            }
        }

        public double? Altitude
        {
            get
            {
                return _altitude;
            }

            set
            {
                _altitude = value;
            }
        }

        public double Longitude
        {
            get
            {
                return _longitude;
            }

            set
            {
                _longitude = value;
            }
        }

        public double Latitude
        {
            get
            {
                return _latitude;
            }

            set
            {
                _latitude = value;
            }
        }

        public string Action
        {
            get
            {
                return _action;
            }

            set
            {
                _action = value;
            }
        }
    }
}
