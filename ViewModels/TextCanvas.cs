using Template10.Mvvm;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class TextCanvas : ObjectCanvas
    {

        string _Text = default(string);
        public string Text { get { return _Text; } set { Set(ref _Text, value); } }

        double _Rotation = default(double);
        public double Rotation { get { return _Rotation; } set { Set(ref _Rotation, value); } }


        Point _Center = default(Point);
        public Point Center { get { return _Center; } set { Set(ref _Center, value); } }
    }

}
