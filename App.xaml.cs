using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Data;

namespace UniversalKeepTheRhythm
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            AppCenter.Start("401bbe20-9a58-4707-8498-9e0e8222a505", typeof(Analytics), typeof(Crashes));
            this.UnhandledException += App_UnhandledException;

            InitializeComponent();
            RequestedTheme = Windows.UI.Xaml.ApplicationTheme.Light;
        }

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.File)
            {
                var tmp = args as FileActivatedEventArgs;
                SessionState.Add("importFile", tmp);

                NavigationService.NavigateAsync(typeof(Views.ImportPage), "importFile");
            }
            else
                NavigationService.NavigateAsync(typeof(Views.MainPage));

            return Task.CompletedTask;
        }

        //        public override void OnFileActivated(
        //  FileActivatedEventArgs args
        //)
        //        {

        //        }

        void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            try
            {

                if (e != null && e.Exception != null)
                {
                    Exception exception = e.Exception;

                    Analytics.TrackEvent("Error", new Dictionary<string, string> {
    { "ExceptionType", e.Exception.ToString() },
    { "ExceptionMessage", e.Exception.Message}
});


                    if ((exception is System.Xml.XmlException || exception is NullReferenceException) && exception.ToString().ToUpper().Contains("INNERACTIVE"))
                    {
                        System.Diagnostics.Debug.WriteLine("Handled Inneractive exception {0}", exception);
                        e.Handled = true;
                        return;
                    }
                    else if ((exception is NullReferenceException || exception is System.IO.FileNotFoundException) && exception.ToString().ToUpper().Contains("SOMA"))
                    {
                        System.Diagnostics.Debug.WriteLine("Handled Smaato null reference exception {0}", exception);
                        e.Handled = true;
                        return;
                    }
                    else if ((exception is System.IO.IOException || exception is NullReferenceException) && exception.ToString().ToUpper().Contains("GOOGLE"))
                    {
                        System.Diagnostics.Debug.WriteLine("Handled Google exception {0}", exception);
                        e.Handled = true;
                        return;
                    }
                    else if (exception is ObjectDisposedException && exception.ToString().ToUpper().Contains("MOBFOX"))
                    {
                        System.Diagnostics.Debug.WriteLine("Handled Mobfox exception {0}", exception);
                        e.Handled = true;
                        return;
                    }
                    else if ((exception is NullReferenceException) && exception.ToString().ToUpper().Contains("MICROSOFT.ADVERTISING"))
                    {
                        System.Diagnostics.Debug.WriteLine("Handled Microsoft.Advertising exception {0}", exception);
                        e.Handled = true;
                        return;
                    }

                }
            }
            catch (Exception)
            {

            }

            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    // An unhandled exception has occurred; break into the debugger
            //    System.Diagnostics.Debugger.Break();
            //}
            //else
            //{
            try
            {
                nowhereman.LittleWatson.instance.ReportException(e.Exception, "UniversalKeepTheRhythm");
            }
            catch { }

            //}
        }

    }
}
