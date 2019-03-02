using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalKeepTheRhythm.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace UniversalKeepTheRhythm.Converters
{
    class CanvasSelector : DataTemplateSelector
    {
        public DataTemplate TextCanvasTemplate { get; set; }
        public DataTemplate LineCanvasTemplate { get; set; }
        public DataTemplate PolylineCanvasTemplate { get; set; }
        public DataTemplate PolygonCanvasTemplate { get; set; }
        public DataTemplate RectangleCanvasTemplate { get; set; }
        public DataTemplate EllipseCanvasTemplate { get; set; }
        public DataTemplate ShowInfoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            var tmp = item as ObjectCanvas;
            if (tmp != null)
            {
                ////Canvas.SetLeft((UIElement)container, tmp.Left);
                ////Canvas.SetTop((UIElement)container, tmp.Top);

                element.SetBinding(Canvas.LeftProperty, new Binding()
                {
                    Path = new PropertyPath("Left"),
                    Source = tmp,
                    Mode = BindingMode.OneWay
                });

                element.SetBinding(Canvas.TopProperty, new Binding()
                {
                    Path = new PropertyPath("Top"),
                    Source = tmp,
                    Mode = BindingMode.OneWay
                });
            }

            if (element != null && item != null && item is TextCanvas)
            {
                return TextCanvasTemplate;
            }
            if (element != null && item != null && item is LineCanvas)
            {
                return LineCanvasTemplate;
            }
            if (element != null && item != null && item is PolygonCanvas)
            {
                return PolygonCanvasTemplate;
            }
            if (element != null && item != null && item is PolylineCanvas)
            {
                return PolylineCanvasTemplate;
            }
            if (element != null && item != null && item is RectangleCanvas)
            {
                return RectangleCanvasTemplate;
            }
            if (element != null && item != null && item is ShowInfoCanvas)
            {
                return ShowInfoTemplate;
            }
            if (element != null && item != null && item is EllipseCanvas)
            {
                Canvas.SetZIndex(element, 1000);
                return EllipseCanvasTemplate;
            }

            return null;
        }
        }
}
