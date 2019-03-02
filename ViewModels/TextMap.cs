using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using Windows.UI.Xaml;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class TextMap: MapObject
    {
        string _Title = default(string);
        public string Title { get { return _Title; } set { Set(ref _Title, value); } }

        public Style StyleText { get; set; }
    }
}
