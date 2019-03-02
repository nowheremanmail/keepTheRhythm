using nowhereman;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UniversalKeepTheRhythm.Services;
using UniversalKeepTheRhythm.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
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
    public sealed partial class ProgressPage : Page
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        //ManipulationInputProcessor manipulator;
        GestureRecognizer gr = new GestureRecognizer();
        MapPolyline lineOnMap = new MapPolyline();

        public ProgressPage()
        {
            this.InitializeComponent();

            //manipulator = new ManipulationInputProcessor(gr, fondo, LayoutRoot);


            this.PointerPressed += MainPage_PointerPressed;
            this.PointerMoved += MainPage_PointerMoved;
            this.PointerReleased += MainPage_PointerReleased;

            gr.CrossSliding += gr_CrossSliding;
            gr.Dragging += gr_Dragging;
            gr.Holding += gr_Holding;
            gr.ManipulationCompleted += gr_ManipulationCompleted;
            gr.ManipulationInertiaStarting += gr_ManipulationInertiaStarting;
            gr.ManipulationStarted += gr_ManipulationStarted;
            gr.ManipulationUpdated += gr_ManipulationUpdated;
            gr.RightTapped += gr_RightTapped;
            gr.Tapped += gr_Tapped;
           
            gr.GestureSettings = Windows.UI.Input.GestureSettings.ManipulationTranslateX | Windows.UI.Input.GestureSettings.ManipulationTranslateY;

            //Windows.UI.Input.GestureSettings.ManipulationRotateInertia | Windows.UI.Input.GestureSettings.ManipulationScaleInertia |
            //Windows.UI.Input.GestureSettings.ManipulationTranslateInertia | Windows.UI.Input.GestureSettings.ManipulationScale | Windows.UI.Input.GestureSettings.ManipulationRotate |


            var data = DataContext as ProgressViewModel;
            var Cur = data.CurrentPoints as ObservableCollection<BasicGeoposition>;

            if (Cur != null)
            {
                Cur.CollectionChanged += Cur_CollectionChanged;
            }

            lineOnMap.StrokeColor = Colors.Orange;
            lineOnMap.StrokeThickness = 4;
            lineOnMap.Path = new Geopath(new List<BasicGeoposition>() { new BasicGeoposition() { Latitude = 41.3825, Longitude = 2.176944, Altitude = 13 } });
            map1.MapElements.Add(lineOnMap);

            data.PropertyChanged += Data_PropertyChanged;
        }

        private  void Data_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var data = DataContext as ProgressViewModel;
            if (e.PropertyName == "LandmarkLayer")
            {
                foreach (MapObject obj in data.LandmarkLayer.FindAll((a) => a is PolylineMap))
                {
                //    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //    {
                        var tmp = obj as PolylineMap;
                        if (tmp != null)
                            map1.MapElements.Add(new MapPolyline() { Path = tmp.MapRoute, StrokeColor = tmp.StrokeColor, StrokeThickness = tmp.StrokeThickness });
                //    });
                }
            }
        }

        void MainPage_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var ps = e.GetIntermediatePoints(null);
            if (ps != null && ps.Count > 0)
            {
                gr.ProcessUpEvent(ps[0]);
                e.Handled = true;
                gr.CompleteGesture();
            }
        }

        void MainPage_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            gr.ProcessMoveEvents(e.GetIntermediatePoints(null));
            e.Handled = true;
        }

        void MainPage_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var ps = e.GetIntermediatePoints(null);
            if (ps != null && ps.Count > 0)
            {
                gr.ProcessDownEvent(ps[0]);
                e.Handled = true;
            }
        }

        private void Cur_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            lineOnMap.Path = new Geopath(sender as ObservableCollection<BasicGeoposition>);
        }

        void gr_Tapped(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.TappedEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_Tapped");
#endif
 
            var data = DataContext as ProgressViewModel;
            data.Tap();
        }
        void gr_RightTapped(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.RightTappedEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_RightTapped");
#endif
        }
        void gr_Holding(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.HoldingEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_Holding");
#endif
            var data = DataContext as ProgressViewModel;
            data.HoldTap();
        }
        void gr_Dragging(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.DraggingEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_Dragging");
#endif
        }
        void gr_CrossSliding(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.CrossSlidingEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_CrossSliding");
#endif
        }
        void gr_ManipulationUpdated(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.ManipulationUpdatedEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_ManipulationUpdated");
#endif
        }
        void gr_ManipulationStarted(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.ManipulationStartedEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_ManipulationStarted");
#endif
        }
            void gr_ManipulationCompleted(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.ManipulationCompletedEventArgs args)
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_ManipulationCompleted");
#endif
            var data = DataContext as ProgressViewModel;

            if (Math.Abs(args.Cumulative.Translation.X) > Math.Abs(args.Cumulative.Translation.Y)) {
                data.flick(Orientation.Horizontal, args.Cumulative.Translation.X);
            }
            else
            {
                data.flick(Orientation.Vertical, args.Cumulative.Translation.Y);
            }
        }
        void gr_ManipulationInertiaStarting(Windows.UI.Input.GestureRecognizer sender, Windows.UI.Input.ManipulationInertiaStartingEventArgs args)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("gr_ManipulationInertiaStarting");
#endif
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.PointerPressed -= MainPage_PointerPressed;
            this.PointerMoved -= MainPage_PointerMoved;
            this.PointerReleased -= MainPage_PointerReleased;

            gr.CrossSliding -= gr_CrossSliding;
            gr.Dragging -= gr_Dragging;
            gr.Holding -= gr_Holding;
            gr.ManipulationCompleted -= gr_ManipulationCompleted;
            gr.ManipulationInertiaStarting -= gr_ManipulationInertiaStarting;
            gr.ManipulationStarted -= gr_ManipulationStarted;
            gr.ManipulationUpdated -= gr_ManipulationUpdated;
            gr.RightTapped -= gr_RightTapped;
            gr.Tapped -= gr_Tapped;

            var data = DataContext as ProgressViewModel;
            var Cur = data.CurrentPoints as ObservableCollection<BasicGeoposition>;

            if (Cur != null)
            {
                Cur.CollectionChanged -= Cur_CollectionChanged;
            }
            data.PropertyChanged -= Data_PropertyChanged;

            base.OnNavigatedFrom(e);
        }


        private void CCC_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
            helpInfo.IsOpen = false;
            var data = DataContext as ProgressViewModel;
            data.start();
        }
        private void map1_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var data = DataContext as ProgressViewModel;
            data.mouseMoved();
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            questionPopup.Width = e.NewSize.Width;
            questionPopup.Height = e.NewSize.Height;

            addPointPopup.Width = e.NewSize.Width;
            addPointPopup.Height = e.NewSize.Height;

            helpInfo.Width = e.NewSize.Width;
            helpInfo.Height = e.NewSize.Height;

        }

  
    }
}