using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class MapObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        Geopoint _Location = default(Geopoint);
        public Geopoint Location { get { return _Location; } set { Set(ref _Location, value); } }
       
        public object Tag { get; set; }

        Style _PanelStyleText = default(Style);
        public Style PanelStyleText { get { return _PanelStyleText; } set { Set(ref _PanelStyleText, value); } }
        //public Style PanelStyleText { get; set; }
    }
}
