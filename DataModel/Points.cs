namespace UniversalKeepTheRhythm.model
{
    public partial class Points
    {

        private int _id;

        private System.Nullable<double> _speed;

        private System.Nullable<double> _altitude;

        private double _longitude;

        private double _latitude;

        private string _name;

        private int _idPath;

        private string _type;

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

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public int IdPath
        {
            get
            {
                return _idPath;
            }

            set
            {
                _idPath = value;
            }
        }
    }
}
