namespace UniversalKeepTheRhythm.model
{
    public partial class PointInterest
    {
        private int _id;

        private double _longitude;

        private double _latitude;

        private int _idSession;

        private long _time;

        private double _distance;

        private string _message;
        private string _type;

        private System.Nullable<double> _rhythm;

        public int Id
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

        public string Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
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

        public long Time
        {
            get
            {
                return _time;
            }

            set
            {
                _time = value;
            }
        }

        public double Distance
        {
            get
            {
                return _distance;
            }

            set
            {
                _distance = value;
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }

            set
            {
                _message = value;
            }
        }

        public double? Rhythm
        {
            get
            {
                return _rhythm;
            }

            set
            {
                _rhythm = value;
            }
        }
    }
}
