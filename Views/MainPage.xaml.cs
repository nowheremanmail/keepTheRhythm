using System;
using System.Collections.Generic;
using Template10.Services.NavigationService;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace UniversalKeepTheRhythm.Views
{
    public sealed partial class MainPage : Page
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

            CCC.NavigateToString(Others.Utils.WrapHtml(resourceLoader.GetString("Conditions"), 500, null));
        }

        private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            conditions.Width = e.NewSize.Width; 
            conditions.Height = e.NewSize.Height;
        }
    }
}