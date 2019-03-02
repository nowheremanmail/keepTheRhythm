using System.ComponentModel;
using System.Runtime.CompilerServices;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace UniversalKeepTheRhythm.ViewModels
{

    public class ObjectCanvas : INotifyPropertyChanged
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



        double _Left = default(double);
        public double Left { get { return _Left; } set { Set(ref _Left, value); } }

        double _Top = default(double);
        public double Top { get { return _Top; } set { Set(ref _Top, value); } }

        public Style StyleText { get; set; }

    }

}
