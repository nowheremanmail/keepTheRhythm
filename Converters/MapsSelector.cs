using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalKeepTheRhythm.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UniversalKeepTheRhythm.Converters
{
    class MapsSelector : DataTemplateSelector
    {
        //https://stackoverflow.com/questions/33252915/how-to-associate-view-with-viewmodel-or-multiple-datatemplates-for-viewmodel/33293716#33293716
        public DataTemplate TextMapTemplate { get; set; }
        public DataTemplate PointMapTemplate { get; set; }
        public DataTemplate LoopTextMapTemplate { get; set; }

        public DataTemplate SpeedPointMapTemplate { get; set; }
        public DataTemplate PacePointMapTemplate { get; set; }
        public DataTemplate EllipseMapTemplate { get; set; }
        public DataTemplate PolylineMapTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is SpeedPointMap)
            {
                return SpeedPointMapTemplate;
            }
            if (item is PacePointMap)
            {
                return PacePointMapTemplate;
            }
            if (item is LoopTextMap)
            {
                return LoopTextMapTemplate;
            }
            if (item is ShowInfoCanvas)
            {
                return PointMapTemplate;
            }
            if (item is TextMap)
            {
                return TextMapTemplate;
            }
            if (item is PolylineMap)
            {
                return PolylineMapTemplate;
            }
            if (item is EllipseMap)
            {
                return EllipseMapTemplate;
            }

            return null;
        }
    }
}
