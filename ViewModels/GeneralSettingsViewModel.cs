
using Microsoft.Toolkit.Uwp.Notifications;
using nowhereman;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Template10.Mvvm;
using UniversalKeepTheRhythm.Services;
using Windows.ApplicationModel.Resources;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    class GeneralSettingsViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        //public Uri RateMe => new Uri("http://aka.ms/template10");


        public async Task reviewAsync()
        {
            string appid = "";
            var uri = new System.Uri("ms-appx:///AppxManifest.xml");
            StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            using (var rastream = await file.OpenReadAsync())
            using (var appManifestStream = rastream.AsStreamForRead())
            {
                using (var reader = XmlReader.Create(appManifestStream, new XmlReaderSettings { IgnoreWhitespace = true, IgnoreComments = true }))
                {
                    var doc = XDocument.Load(reader);
                    var app = doc.Descendants().Where(e => e.Name.LocalName == "PhoneIdentity").FirstOrDefault();
                    if (app != null)
                    {
                        var idAttribute = app.Attribute("PhoneProductId");
                        if (idAttribute != null)
                        {
                            appid = idAttribute.Value;
                        }
                    }
                }
            }

            var uriM = new Uri(string.Format("ms-windows-store:reviewapp?appid={0}", appid));
            await Windows.System.Launcher.LaunchUriAsync(uriM);
        }


        DelegateCommand _FeedbackCommand;
        public DelegateCommand FeedbackCommand
            => _FeedbackCommand ?? (_FeedbackCommand = new DelegateCommand(async () =>
            {
                var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
                await launcher.LaunchAsync();
            }));

        DelegateCommand _RestoreCommand;
        public DelegateCommand RestoreCommand
            => _RestoreCommand ?? (_RestoreCommand = new DelegateCommand(async () =>
            {
                restoreAction();
            }));


        public bool ShowGraphSummary
        {
            get { return nowhereman.Properties.getBoolProperty("showGraph", true); }
            set { nowhereman.Properties.setBoolProperty("showGraph", value); }
        }

        public bool StoreAll
        {
            get { return nowhereman.Properties.getBoolProperty("storeAllPoints", false); }
            set { nowhereman.Properties.setBoolProperty("storeAllPoints", value); }
        }

        bool _liveTile = default(bool);
        public bool liveTile { get { return _liveTile; } set { Set(ref _liveTile, value); } }

        public bool expertMode
        {
            get { return nowhereman.Properties.getBoolProperty("expertMode", false); }
            set { nowhereman.Properties.setBoolProperty("expertMode", value); }
        }

        IEnumerable<VoiceInformation> _voices = default(IEnumerable<VoiceInformation>);
        public IEnumerable<VoiceInformation> voices { get { return _voices; } set { Set(ref _voices, value); } }


        PairCodeDesc _CurrentUnit = default(PairCodeDesc);
        public PairCodeDesc CurrentUnit
        {
            get { return _CurrentUnit; }
            set
            {
                Set(ref _CurrentUnit, value);
                if (value != null) nowhereman.Properties.setProperty("units", value.code);
            }
        }


        PairCodeDesc _CurrentCartoMode = default(PairCodeDesc);
        public PairCodeDesc CurrentCartoMode
        {
            get { return _CurrentCartoMode; }
            set
            {
                Set(ref _CurrentCartoMode, value);
                if (value != null) nowhereman.Properties.setProperty("CartoMode", value.code);
            }
        }

        PairCodeDesc _CurrentGraphType = default(PairCodeDesc);
        public PairCodeDesc CurrentGraphType
        {
            get { return _CurrentGraphType; }
            set
            {
                Set(ref _CurrentGraphType, value);
                if (value != null) nowhereman.Properties.setProperty("graphType", value.code);
            }
        }


        PairCodeDesc _CurrentGraphInfo = default(PairCodeDesc);
        public PairCodeDesc CurrentGraphInfo
        {
            get { return _CurrentGraphInfo; }
            set
            {
                Set(ref _CurrentGraphInfo, value);
                if (value != null) nowhereman.Properties.setProperty("graphInfo", value.code);
            }
        }

        PairCodeDesc _CurrentShareFormat = default(PairCodeDesc);
        public PairCodeDesc CurrentShareFormat
        {
            get { return _CurrentShareFormat; }
            set
            {
                Set(ref _CurrentShareFormat, value);
                if (value != null) nowhereman.Properties.setProperty("shareFormat", value.code);
            }
        }

        IEnumerable<PairCodeDesc> _ShareFormat = default(IEnumerable<PairCodeDesc>);
        public IEnumerable<PairCodeDesc> ShareFormats { get { return _ShareFormat; } set { Set(ref _ShareFormat, value); } }

        IEnumerable<PairCodeDesc> _CartoModes = default(IEnumerable<PairCodeDesc>);
        public IEnumerable<PairCodeDesc> CartoModes { get { return _CartoModes; } set { Set(ref _CartoModes, value); } }


        IEnumerable<PairCodeDesc> _GraphTypes = default(IEnumerable<PairCodeDesc>);
        public IEnumerable<PairCodeDesc> GraphTypes { get { return _GraphTypes; } set { Set(ref _GraphTypes, value); } }


        IEnumerable<PairCodeDesc> _GraphInfos = default(IEnumerable<PairCodeDesc>);
        public IEnumerable<PairCodeDesc> GraphInfos { get { return _GraphInfos; } set { Set(ref _GraphInfos, value); } }

        bool _AcceptConditions = nowhereman.Properties.getBoolProperty("acceptedconditions", false);
        public bool AcceptConditions
        {
            get { return _AcceptConditions; }
            set
            {
                Set(ref _AcceptConditions, value);
                 nowhereman.Properties.setBoolProperty("acceptedconditions", value);
            }
        }

        IEnumerable<PairCodeDesc> _Units = default(IEnumerable<PairCodeDesc>);
        public IEnumerable<PairCodeDesc> Units
        {
            get { return _Units; }
            set
            {
                Set(ref _Units, value);
            }
        }


        string _liveTileUpdateTxt = default(string);
        public string liveTileUpdateTxt { get { return _liveTileUpdateTxt; } set { Set(ref _liveTileUpdateTxt, value); } }

        int _liveTileUpdate = default(int);
        public int liveTileUpdate { get { return _liveTileUpdate; } set { Set(ref _liveTileUpdate, value); } }


        private async Task restoreAction()
        {
            try
            {
                LiveApi liveApi;
                liveApi = new LiveApi("keepTheRhythm", null);
                liveApi.notifMessage += LiveApi_notifMessage;

                var tmpFile = await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync("backup_KeepTheRhythm.db", CreationCollisionOption.ReplaceExisting);
                var res = await liveApi.downloadFileAsync("backup_KeepTheRhythm.db", tmpFile);

                if (res == "OK")
                {
                    DataBaseManager newDB = new DataBaseManager("backup_KeepTheRhythm", Windows.Storage.ApplicationData.Current.TemporaryFolder);
                    if (!newDB.CheckDataBase())
                    {
                        throw new Exception(resourceLoader.GetString("EmptyDatabase"));
                    }
                    DataBaseManager.instance.Close();
                    newDB.Close();

                    StorageFile file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.GetFileAsync("backup_KeepTheRhythm.db");
                    await file.MoveAsync(Windows.Storage.ApplicationData.Current.LocalFolder, "ktr.db", NameCollisionOption.ReplaceExisting);

                    showNotif(resourceLoader.GetString("endRestore"), resourceLoader.GetString("attention"));
                }
            }
            catch (Exception ee)
            {
                showNotif(string.Format(resourceLoader.GetString("endKORestore"), ee.Message), resourceLoader.GetString("attention"));
                nowhereman.LittleWatson.instance.ReportException(ee, "restore");
            }

        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            string tmp;

            //liveApi.start();

            liveTile = nowhereman.Properties.getBoolProperty("updateTile", true);


            liveTileUpdate = nowhereman.Properties.getIntProperty("refreshTile", 30);
            liveTileUpdateTxt = string.Format(resourceLoader.GetString("liveTileUpdate"), liveTileUpdate);

            var listCol = new ObservableCollection<VoiceInformation>();

            //foreach (var voice in InstalledVoices.All)
            //{
            //    listCol.Add(voice);
            //}

            voices = listCol;

            ObservableCollection<PairCodeDesc> shareFormats = new ObservableCollection<PairCodeDesc>();
            shareFormats.Add(new PairCodeDesc("kml", resourceLoader.GetString("Kml")));
            shareFormats.Add(new PairCodeDesc("gpx", resourceLoader.GetString("Gpx")));

            ShareFormats = shareFormats;

            tmp = nowhereman.Properties.getProperty("shareFormat", "kml");

            CurrentShareFormat = shareFormats[shareFormats.IndexOf(new PairCodeDesc(tmp, ""))];


            ObservableCollection<PairCodeDesc> listActions = new ObservableCollection<PairCodeDesc>();
            listActions.Add(new PairCodeDesc(MapStyle.Aerial.ToString(), resourceLoader.GetString("CartoModeAerial")));
            listActions.Add(new PairCodeDesc(MapStyle.AerialWithRoads.ToString(), resourceLoader.GetString("CartoModeHybrid")));
            listActions.Add(new PairCodeDesc(MapStyle.Road.ToString(), resourceLoader.GetString("CartoModeRoad")));
            listActions.Add(new PairCodeDesc(MapStyle.Terrain.ToString(), resourceLoader.GetString("CartoModeTerrain")));

            CartoModes = listActions;
            tmp = nowhereman.Properties.getProperty("CartoMode", MapStyle.Road.ToString());

            //cartoMode.ItemsSource = listActions;
            CurrentCartoMode = listActions[listActions.IndexOf(new PairCodeDesc(tmp, ""))];



            ObservableCollection<PairCodeDesc> listUnits = new ObservableCollection<PairCodeDesc>();
            listUnits.Add(new PairCodeDesc("m", resourceLoader.GetString("Metric")));
            listUnits.Add(new PairCodeDesc("mi", resourceLoader.GetString("Imperial")));

            Units = listUnits;

            tmp = nowhereman.Properties.getProperty("units", "m");

            //units.ItemsSource = listUnits;
            CurrentUnit = listUnits[listUnits.IndexOf(new PairCodeDesc(tmp, ""))];




            ObservableCollection<PairCodeDesc> listGraphInfo = new ObservableCollection<PairCodeDesc>();
            listGraphInfo.Add(new PairCodeDesc("td", resourceLoader.GetString("GraphInfo0")));
            listGraphInfo.Add(new PairCodeDesc("t", resourceLoader.GetString("GraphInfo1")));
            listGraphInfo.Add(new PairCodeDesc("d", resourceLoader.GetString("GraphInfo2")));

            GraphInfos = listGraphInfo;

            tmp = nowhereman.Properties.getProperty("graphInfo", "td");

            //graphInfo.ItemsSource = listGraphInfo;
            CurrentGraphInfo = listGraphInfo[listGraphInfo.IndexOf(new PairCodeDesc(tmp, ""))];



            ObservableCollection<PairCodeDesc> listGraphType = new ObservableCollection<PairCodeDesc>();
            listGraphType.Add(new PairCodeDesc("D", resourceLoader.GetString("GraphType0")));
            listGraphType.Add(new PairCodeDesc("W", resourceLoader.GetString("GraphType1")));
            listGraphType.Add(new PairCodeDesc("M", resourceLoader.GetString("GraphType2")));
            listGraphType.Add(new PairCodeDesc("Y", resourceLoader.GetString("GraphType3")));

            tmp = nowhereman.Properties.getProperty("graphType", "D");

            GraphTypes = listGraphType;
            CurrentGraphType = listGraphType[listGraphType.IndexOf(new PairCodeDesc(tmp, ""))];


            //ObservableCollection<PairCodeDesc> listRadioFM = new ObservableCollection<PairCodeDesc>();
            //listRadioFM.Add(new PairCodeDesc(Microsoft.Devices.Radio.RadioRegion.Europe.ToString(), resourceLoader.GetString("Europe")));
            //listRadioFM.Add(new PairCodeDesc(Microsoft.Devices.Radio.RadioRegion.UnitedStates.ToString(), resourceLoader.GetString("UnitedStates")));
            //listRadioFM.Add(new PairCodeDesc(Microsoft.Devices.Radio.RadioRegion.Japan.ToString(), resourceLoader.GetString("Japan")));

            //tmp = nowhereman.Properties.getProperty("radioCurrentRegion", Microsoft.Devices.Radio.RadioRegion.Europe.ToString());

            //radioFMMode.ItemsSource = listRadioFM;
            //radioFMMode.SelectedItem = listRadioFM[listRadioFM.IndexOf(new PairCodeDesc(tmp, ""))];

            return Task.CompletedTask;
        }



        private void LiveApi_notifMessage(LiveApi sender, string msg, Exception ex)
        {

        }

        //        private async void LiveApi_uploadFinished(LiveApi sender, LiveUploadOperation oper, LiveOperationResult res)
        //        {

        //            try
        //            {
        //#if (DEBUG)
        //                System.Diagnostics.Debug.WriteLine("UPLOAD! path  " + res.Result["name"]);
        //#endif
        //                string name = res.Result["name"] as string;
        //                if (name.EndsWith(".db"))
        //                {
        //                    try
        //                    {
        //                        showNotif(string.Format(resourceLoader.GetString("UploadSuccessBack"), "backup_KeepTheRhythm.db"), resourceLoader.GetString("attention"));

        //                        StorageFile newfile = await Windows.Storage.ApplicationData.Current.TemporaryFolder.GetFileAsync("backup_KeepTheRhythm.db");
        //                        await newfile.DeleteAsync();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        showNotif(string.Format(resourceLoader.GetString("UploadUnSuccess"), "backup_KeepTheRhythm.db", ex.Message), resourceLoader.GetString("attention"));
        //                        nowhereman.LittleWatson.instance.ReportException(ex, "backup");
        //                    }
        //                }

        //                //Analytics.TrackEvent("Upload", new Dictionary<string, string> {
        //                //        { "Argument", name }
        //                //    });

        //            }
        //            catch (Exception ex)
        //            {
        //                nowhereman.LittleWatson.instance.ReportException(ex, oper.ToString());
        //            }
        //        }


        private void showNotif(string v1, string v2)
        {
            Show(new ToastContent()
            {
                Launch = "app-defined-string",
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children = {
                                            new AdaptiveText()
                                            {

                                                            Text = "Keep The Rhythm - " + v2
                                            },

                                            new AdaptiveText()
                                            {
                                                Text = v1
                                            }
                                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons = { new ToastButtonDismiss() }
                }
            });
        }

        private void Show(ToastContent content)
        {
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }

    }
}
