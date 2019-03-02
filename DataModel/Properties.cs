using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Data.Sqlite;

namespace UniversalKeepTheRhythm.model
{

    public partial class Properties
    {

        private string _key;

        private string _value;

        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
            }
        }
    }


}
