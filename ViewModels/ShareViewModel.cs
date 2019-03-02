using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Services;
using UniversalKeepTheRhythm.Others;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    class ShareViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();


        DelegateCommand _GpxCommand;
        public DelegateCommand GpxCommand
           => _GpxCommand ?? (_GpxCommand = new DelegateCommand(() =>
           {
               share("gpx");
           }, () => true));


        DelegateCommand _KmlCommand;
        public DelegateCommand KmlCommand
           => _KmlCommand ?? (_KmlCommand = new DelegateCommand(() =>
           {
               share("kml");
           }, () => true));

        int idSession;
        string mode;
        Sessions session;
        Paths path;
        string format;

        public override Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= ShareViewModel_DataRequested;

            return Task.CompletedTask;
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {
            DataTransferManager.GetForCurrentView().DataRequested += ShareViewModel_DataRequested; 

            if (modeNav == NavigationMode.New)
            {
                idSession = (int)parameter;
                //var paramsTo = parameter as Dictionary<String, object>;

                //if (paramsTo.ContainsKey("idSession"))
                //{
                //    idSession = (int)((Int64)paramsTo["idSession"]);

                //}

                session = DataBaseManager.instance.GetSession(idSession);
                path = DataBaseManager.instance.GetPath(session.IdPath);

                mode = path.Type;
                //mode = paramsTo["mode"] as string;


            }

            return Task.CompletedTask;
        }

        void share (string format)
        {
            this.format = format;
            DataTransferManager.ShowShareUI();
        
        }

        private async void ShareViewModel_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequestDeferral deferral = args.Request.GetDeferral();
            try
            {
                var request = args.Request;

                request.Data.Properties.Title = path.Description + " @ " + session.DayOfSession;
                request.Data.Properties.Description = path.Description + " @ " + session.DayOfSession;

                List<IStorageItem> l = new List<IStorageItem>();
                String unit = nowhereman.Properties.getProperty("units", "m");

                StorageFile file = format == "gpx"? await UniversalKeepTheRhythm.Others.ExportImport.exportGpx(idSession, "route.gpx", unit, resourceLoader)
                : await UniversalKeepTheRhythm.Others.ExportImport.exportKml(idSession, "route.kml", unit, resourceLoader);

                //RandomAccessStreamReference image = null;
                //image = RandomAccessStreamReference.CreateFromFile(file);
                l.Add(file);

                //request.Data.Properties.Thumbnail = image;
                //request.Data.SetBitmap(image);

                request.Data.SetStorageItems(l);
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
