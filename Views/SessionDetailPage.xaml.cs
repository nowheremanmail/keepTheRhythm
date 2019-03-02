using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UniversalKeepTheRhythm.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UniversalKeepTheRhythm.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SessionDetailPage : Page
    {
        public SessionDetailPage()
        {
            this.InitializeComponent();

            var data = DataContext as SessionDetailViewModel;
            data.PropertyChanged += Data_PropertyChanged;
        }

        private void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var data = DataContext as SessionDetailViewModel;
            if (e.PropertyName == "LandmarkLayer")
            {
                foreach (MapObject obj in data.LandmarkLayer.FindAll((a) => a is PolylineMap))
                {
                    var tmp = obj as PolylineMap;
                    if (tmp != null)
                        map1.MapElements.Add(new MapPolyline() { Path = tmp.MapRoute, StrokeColor = tmp.StrokeColor, StrokeThickness = tmp.StrokeThickness });
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var data = DataContext as SessionDetailViewModel;
            data.PropertyChanged -= Data_PropertyChanged;

            base.OnNavigatedFrom(e);
        }

        private void Canvas_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var data = DataContext as SessionDetailViewModel;
            if (data != null)
            {
                data.Canvas_Tapped(sender, e);
            }
        }

        private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var data = DataContext as SessionDetailViewModel;
            if (data != null)
            {
                data.Canvas_PointerMoved(sender, e);
            }
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var data = DataContext as SessionDetailViewModel;
            if (data != null)
            {
                data.SizeChanged(sender, e);
            }
        }
    }
}
