using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Services;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media;
using Windows.Devices.Sensors;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using System.Threading;
using nowhereman;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Input;
using UniversalKeepTheRhythm.Others;
using Windows.UI;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class ProgressViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        DelegateCommand _FollowAction;
        public DelegateCommand FollowAction
           => _FollowAction ?? (_FollowAction = new DelegateCommand(() =>
           {
               FollowCommand();
           }, () => true));


        DelegateCommand _CompassAction;
        public DelegateCommand CompassAction
           => _CompassAction ?? (_CompassAction = new DelegateCommand(() =>
           {
               throw new NotImplementedException();
           }, () => true));


        DelegateCommand _MapTypeAction;
        public DelegateCommand MapTypeAction
           => _MapTypeAction ?? (_MapTypeAction = new DelegateCommand(() =>
           {
               MapTypeCommand();
           }, () => true));


        DelegateCommand _MapLightAction;
        public DelegateCommand MapLightAction
           => _MapLightAction ?? (_MapLightAction = new DelegateCommand(() =>
           {
               MapLightCommand();
           }, () => true));


        DelegateCommand _RotateAction;
        public DelegateCommand RotateAction
           => _RotateAction ?? (_RotateAction = new DelegateCommand(() =>
           {
               throw new NotImplementedException();
           }, () => true));


        DelegateCommand _LocateAction;
        public DelegateCommand LocateAction
           => _LocateAction ?? (_LocateAction = new DelegateCommand(() =>
           {
               LocateCommand();
           }, () => true));

        DelegateCommand _StartAction;
        public DelegateCommand StartAction
           => _StartAction ?? (_StartAction = new DelegateCommand(() =>
           {
               StartCommand(true);
           }, () => true));

        DelegateCommand _StopAction;
        public DelegateCommand StopAction
           => _StopAction ?? (_StopAction = new DelegateCommand(() =>
           {
               StopCommand(true);
           }, () => true));

        DelegateCommand _PauseAction;
        public DelegateCommand PauseAction
           => _PauseAction ?? (_PauseAction = new DelegateCommand(() =>
           {
               PauseCommand(true);
           }, () => true));

        DelegateCommand _ResumeAction;
        public DelegateCommand ResumeAction
           => _ResumeAction ?? (_ResumeAction = new DelegateCommand(() =>
           {
               ResumeCommand(true);
           }, () => true));

        private ScreenStatus _Status = new ScreenStatus();
        public ScreenStatus Status { get { return _Status; } set { Set(ref _Status, value); } }


        TappedEventHandler _HelpInfoClosed ;
        public TappedEventHandler HelpInfoClose
           => _HelpInfoClosed ?? (_HelpInfoClosed = new TappedEventHandler((sender,args) =>
           {
               HelpInfo = false;
               start(); 
           }));


        EventHandler<object> _QuestionClosed;
        public EventHandler<object> QuestionClosed
           => _QuestionClosed ?? (_QuestionClosed = new EventHandler<object>((sender, args) =>
           {
               questionPopup_Closed();
           }));

        EventHandler<object> _QuestionOpened;
        public EventHandler<object> QuestionOpened
           => _QuestionOpened ?? (_QuestionOpened = new EventHandler<object>((sender, args) =>
           {
               questionPopup_Opened();
           }));


        EventHandler<object> _AddPointClosed;
        public EventHandler<object> AddPointClosed
           => _AddPointClosed ?? (_AddPointClosed = new EventHandler<object>((sender, args) =>
           {
               addPointPopup_Closed();
           }));

        EventHandler<object> _AddPointOpened;
        public EventHandler<object> AddPointOpened
           => _AddPointOpened ?? (_AddPointOpened = new EventHandler<object>((sender, args) =>
           {
               addPointPopup_Opened();
           }));

        bool _QuestionPopup = false;
        public bool QuestionPopup
        {
            get { return _QuestionPopup; }
            set
            {
                Set(ref _QuestionPopup, value);
            }
        }

        ShowInfoCanvas _ShowInfo = new ShowInfoCanvas();
        public ShowInfoCanvas ShowInfo { get { return _ShowInfo; } set { Set(ref _ShowInfo, value); } }

        List<object> _LandmarkLayer = default(List<object>);
        public List<object> LandmarkLayer { get { return _LandmarkLayer; } set { Set(ref _LandmarkLayer, value); } }

        string _MapServiceToken = "CXLf4gyo8k1gFsG3kyF5~zhvmsbdiZw22us21grWyig~AnJC7VCIbiG4zKbYYwFJg9UKyf2zOwbzdQDNOtZfcDyGXl5NVqOpGnG1Oq3Hu420";
        public string MapServiceToken { get { return _MapServiceToken; } set { Set(ref _MapServiceToken, value); } }

        bool _PointPopup = false;
        public bool PointPopup { get { return _PointPopup; } set { Set(ref _PointPopup, value); } }

        Boolean _HelpInfo = false;
        public Boolean HelpInfo
        {
            get { return _HelpInfo; }
            set
            {
                Set(ref _HelpInfo, value);
            }
        }

        Style _C0Style = Application.Current.Resources["PAUSE_OFF"] as Style;
        public Style C0Style { get { return _C0Style; } set { Set(ref _C0Style, value); } }

        Style _C1Style = Application.Current.Resources["CONTINUE_OFF"] as Style;
        public Style C1Style { get { return _C1Style; } set { Set(ref _C1Style, value); } }

        Visibility _ctlsVisibility = default(Visibility);
        public Visibility ctlsVisibility { get { return _ctlsVisibility; } set { Set(ref _ctlsVisibility, value); } }

        Visibility _MapVisibility = Visibility.Collapsed;
        public Visibility MapVisibility { get { return _MapVisibility; } set { Set(ref _MapVisibility, value); } }

        string _timePP = default(string);
        public string timePP { get { return _timePP; } set { Set(ref _timePP, value); } }

        string _msgPP = default(string);
        public string msgPP { get { return _msgPP; } set { Set(ref _msgPP, value); } }

        string _Dist = default(string);
        public string Dist { get { return _Dist; } set { Set(ref _Dist, value); } }

        string _TxtCurrentTrack = default(string);
        public string TxtCurrentTrack { get { return _TxtCurrentTrack; } set { Set(ref _TxtCurrentTrack, value); } }

        Style _RthymthmRectStyle = Application.Current.Resources["RTHYTHM_KO"] as Style;
        public Style RthymthmRectStyle { get { return _RthymthmRectStyle; } set { Set(ref _RthymthmRectStyle, value); } }

        int _CurrentMapZoomLevel = 18;
        public int CurrentMapZoomLevel
        {
            get { return _CurrentMapZoomLevel; }
            set
            {
                if (Set(ref _CurrentMapZoomLevel, value))
                {
                    if (accuracyCur > 0)
                    {
                        var poit = Ellipse.Location;
                        double metersPerPixels = (Math.Cos(poit.Position.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, value));
                        double radius = accuracyCur / metersPerPixels;

                        Ellipse.Width = radius * 2;
                        Ellipse.Height = radius * 2;
                    }

                }
            }
        }

        MapStyle _CurrentMapStyle = MapStyle.Road;
        public MapStyle CurrentMapStyle { get { return _CurrentMapStyle; } set { Set(ref _CurrentMapStyle, value); } }



        internal void mouseMoved()
        {
            followMap = false;
            FollowVisibility = Visibility.Visible;
        }

        internal void addPointPopup_Opened()
        {
            throw new NotImplementedException();
        }

        internal void addPointPopup_Closed()
        {
            throw new NotImplementedException();
        }



        EllipseMap _Ellipse = new EllipseMap();
        public EllipseMap Ellipse { get { return _Ellipse; } set { Set(ref _Ellipse, value); } }

        Geopoint _MapCenter = default(Geopoint);
        public Geopoint MapCenter { get { return _MapCenter; } set { Set(ref _MapCenter, value); } }

        internal void MapZoomLevelChanged()
        {
            if (accuracyCur > 0)
            {
                var poit = Ellipse.Location;
                double metersPerPixels = (Math.Cos(poit.Position.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, CurrentMapZoomLevel));
                double radius = accuracyCur / metersPerPixels;

                Ellipse.Width = radius * 2;
                Ellipse.Height = radius * 2;
            }
        }

        string _C0 = default(string);
        public string C0 { get { return _C0; } set { Set(ref _C0, value); } }

        string _C1 = default(string);
        public string C1 { get { return _C1; } set { Set(ref _C1, value); } }

        string _Info = default(string);
        public string Info { get { return _Info; } set { Set(ref _Info, value); } }

        double _Distance = default(double);
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        TimeSpan _Time = default(TimeSpan);
        public TimeSpan Time { get { return _Time; } set { Set(ref _Time, value); } }

        double _Speed = default(double);
        public double Speed { get { return _Speed; } set { Set(ref _Speed, value); } }

        double _Altitude = default(double);
        public double Altitude { get { return _Altitude; } set { Set(ref _Altitude, value); } }

        double _Ascendent = default(double);
        public double Ascendent { get { return _Ascendent; } set { Set(ref _Ascendent, value); } }

        double _Descendent = default(double);
        public double Descendent { get { return _Descendent; } set { Set(ref _Descendent, value); } }


        bool _StartEnabled = false;
        public bool StartEnabled { get { return _StartEnabled; } set { Set(ref _StartEnabled, value); } }


        Visibility _StatusRect1Visibility = Visibility.Visible;
        public Visibility StatusRect1Visibility { get { return _StatusRect1Visibility; } set { Set(ref _StatusRect1Visibility, value); } }

        Style _StatusRect1Style = default(Style);
        public Style StatusRect1Style { get { return _StatusRect1Style; } set { Set(ref _StatusRect1Style, value); } }

        Visibility _WaitText1Visibility = Visibility.Visible;
        public Visibility WaitText1Visibility { get { return _WaitText1Visibility; } set { Set(ref _WaitText1Visibility, value); } }

        Style _WaitText1Style = default(Style);
        public Style WaitText1Style { get { return _WaitText1Style; } set { Set(ref _WaitText1Style, value); } }


        MapColorScheme _MapColor = default(MapColorScheme);
        public MapColorScheme MapColor { get { return _MapColor; } set { Set(ref _MapColor, value); } }

        string _WaitText1 = default(string);
        public string WaitText1 { get { return _WaitText1; } set { Set(ref _WaitText1, value); } }


        string _WaitText2 = default(string);
        public string WaitText2 { get { return _WaitText2; } set { Set(ref _WaitText2, value); } }

        string _WaitText3 = default(string);
        public string WaitText3 { get { return _WaitText3; } set { Set(ref _WaitText3, value); } }

        Visibility _StatusRect2Visibility = Visibility.Visible;
        public Visibility StatusRect2Visibility { get { return _StatusRect2Visibility; } set { Set(ref _StatusRect2Visibility, value); } }

        Style _StatusRect2Style = default(Style);
        public Style StatusRect2Style { get { return _StatusRect2Style; } set { Set(ref _StatusRect2Style, value); } }

        Visibility _WaitText2Visibility = Visibility.Visible;
        public Visibility WaitText2Visibility { get { return _WaitText2Visibility; } set { Set(ref _WaitText2Visibility, value); } }

        Style _WaitText2Style = default(Style);
        public Style WaitText2Style { get { return _WaitText2Style; } set { Set(ref _WaitText2Style, value); } }


        Visibility _StatusRect3Visibility = Visibility.Visible;
        public Visibility StatusRect3Visibility { get { return _StatusRect3Visibility; } set { Set(ref _StatusRect3Visibility, value); } }

        Style _StatusRect3Style = default(Style);
        public Style StatusRect3Style { get { return _StatusRect3Style; } set { Set(ref _StatusRect3Style, value); } }

        Visibility _WaitText3Visibility = Visibility.Visible;
        public Visibility WaitText3Visibility { get { return _WaitText3Visibility; } set { Set(ref _WaitText3Visibility, value); } }

        Style _WaitText3Style = default(Style);
        public Style WaitText3Style { get { return _WaitText3Style; } set { Set(ref _WaitText3Style, value); } }

        Visibility _ResumeVisibility = Visibility.Collapsed;
        public Visibility ResumeVisibility { get { return _ResumeVisibility; } set { Set(ref _ResumeVisibility, value); } }


        Visibility _StartVisibility = Visibility.Collapsed;
        public Visibility StartVisibility { get { return _StartVisibility; } set { Set(ref _StartVisibility, value); } }


        Visibility _StopVisibility = Visibility.Visible;
        public Visibility StopVisibility { get { return _StopVisibility; } set { Set(ref _StopVisibility, value); } }


        Visibility _PauseVisibility = Visibility.Visible;
        public Visibility PauseVisibility { get { return _PauseVisibility; } set { Set(ref _PauseVisibility, value); } }

        Visibility _FollowVisibility = Visibility.Collapsed;
        public Visibility FollowVisibility { get { return _FollowVisibility; } set { Set(ref _FollowVisibility, value); } }

        Visibility _CompassVisibility = Visibility.Collapsed;
        public Visibility CompassVisibility { get { return _CompassVisibility; } set { Set(ref _CompassVisibility, value); } }

        Visibility _LocatedVisibility = Visibility.Visible;
        public Visibility LocatedVisibility { get { return _LocatedVisibility; } set { Set(ref _LocatedVisibility, value); } }

        Visibility _RotateVisibility = Visibility.Visible;
        public Visibility RotateVisibility { get { return _RotateVisibility; } set { Set(ref _RotateVisibility, value); } }


        Visibility _MapTypeVisibility = Visibility.Collapsed;
        public Visibility MapTypeVisibility { get { return _MapTypeVisibility; } set { Set(ref _MapTypeVisibility, value); } }

        Visibility _MapLightVisibility = Visibility.Visible;
        public Visibility MapLightVisibility { get { return _MapLightVisibility; } set { Set(ref _MapLightVisibility, value); } }


        Visibility _SpeedCanvasVisibility = default(Visibility);
        public Visibility SpeedCanvasVisibility { get { return _SpeedCanvasVisibility; } set { Set(ref _SpeedCanvasVisibility, value); } }


        Visibility _FondoVisibility = default(Visibility);
        public Visibility FondoVisibility { get { return _FondoVisibility; } set { Set(ref _FondoVisibility, value); } }

        Style _SpeedStyle = Application.Current.Resources["OK_STYLE"] as Style;
        public Style SpeedStyle { get { return _SpeedStyle; } set { Set(ref _SpeedStyle, value); } }


        string _Action0 = default(string);
        public string Action0 { get { return _Action0; } set { Set(ref _Action0, value); } }

        string _Action1 = default(string);
        public string Action1 { get { return _Action1; } set { Set(ref _Action1, value); } }

        string _Action2 = default(string);
        public string Action2 { get { return _Action2; } set { Set(ref _Action2, value); } }

        string _Action3 = default(string);
        public string Action3 { get { return _Action3; } set { Set(ref _Action3, value); } }

        string _Action4 = default(string);
        public string Action4 { get { return _Action4; } set { Set(ref _Action4, value); } }

        string _Action5 = default(string);
        public string Action5 { get { return _Action5; } set { Set(ref _Action5, value); } }

        string _Action6 = default(string);
        public string Action6 { get { return _Action6; } set { Set(ref _Action6, value); } }

        string _mode = "R";
        public string mode { get { return _mode; } set { Set(ref _mode, value); } }

        double accuracyCur = -1;

        int idPath = 1;
        int idSession = -1;

        string desc = "";
        //double margin;

        private Timer popupTimer = null;
        private int timerN = 0;

        bool expertMode = true;

        bool hasMusic = true;
        bool hasRadio = false;
        //bool hasCanvas = false;
        //bool hasMetronom = false;
        bool hasGPS = true;
        bool hasAdvices = true;
        bool modeDay = true;
        bool followCompass = false;
        bool followMap = true;

        bool hasTile;
        bool isRotated = false;
        bool modePace = true;
        int stack = 0;
        int timeForQuestion = 10;
        double MAX_ACCURACY = 50;
        bool requiredAccuracyTime = false;
        double requiredAccuracy;

        private long refresh = -1;
        Geocoordinate lastKnownPosition = null, lastReceivedPosition = null;
        double lastDistance = 0, lastSpeed = 0, lastAltitude = 0, lastPace = 0;
        TimeSpan lastTime = TimeSpan.Zero;
        PointInterest lastPoint = null;

    
        Collection<BasicGeoposition> _CurrentPoints = new ObservableCollection<BasicGeoposition>();
        public Collection<BasicGeoposition> CurrentPoints { get { return _CurrentPoints; } set { Set(ref _CurrentPoints, value); } }

        Brush speedCanvas3Background;
        double speedCanvas3Opacity;
        Compass compass;

        public bool RunningInBackground { get { return false; } }

        List<object> mapElements = new List<object>();

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {
            if (modeNav == NavigationMode.New)
            {
                ShowInfo.Location = new Geopoint(new BasicGeoposition() { Latitude = 41.3825, Longitude = 2.176944, Altitude = 13 });
                Ellipse.Location = new Geopoint(new BasicGeoposition() { Latitude = 41.3825, Longitude = 2.176944, Altitude = 13 });
                mapElements.Add(ShowInfo);
                mapElements.Add(Ellipse);

                var paramsTo = parameter as Dictionary<String, object>;

                if (paramsTo.ContainsKey("idSession"))
                {
                    idSession = (int)((Int64)paramsTo["idSession"]);

                    loadFollowSession(idSession);
                }

                mode = paramsTo["mode"] as string;

                idPath = (int)((Int64)paramsTo["id"]);
                desc = paramsTo["text"] as string;

                if (paramsTo.ContainsKey("followSessionId"))
                {
                    loadFollowSession((int)((Int64)paramsTo["followSessionId"]));
                }

                //margin = nowhereman.Properties.getDoubleProperty("IntervalForLimit" + mode, 0.1);
                timeForQuestion = nowhereman.Properties.getIntProperty("timeForQuestion" + mode, 8);

                //if (mode == "W")
                //{
                //    lastRow.Height = new GridLength(15, GridUnitType.Star);
                //}


                MAX_ACCURACY = nowhereman.Properties.getDoubleProperty("maxAccuracy" + mode, MAX_ACCURACY);

                modeDay = nowhereman.Properties.getBoolProperty("modeDay", true);

                hasTile = nowhereman.Properties.getBoolProperty("updateTile", true);

                expertMode = nowhereman.Properties.getBoolProperty("expertMode", false);

                hasMusic = nowhereman.Properties.getBoolProperty("hasMusic" + mode, true);
                hasRadio = nowhereman.Properties.getBoolProperty("hasRadio" + mode, false);
                modePace = nowhereman.Properties.getBoolProperty("modePace" + mode, true);
                //hasCanvas = nowhereman.Properties.getBoolProperty("hasCanvas" + mode, false);
                //hasMetronom = false; // nowhereman.Properties.getBoolProperty("hasMetronom", false);
                hasGPS = nowhereman.Properties.getBoolProperty("hasGPS" + mode, true);
                hasAdvices = nowhereman.Properties.getBoolProperty("hasAdvices" + mode, true);

                followCompass = nowhereman.Properties.getBoolProperty("followCompass" + mode, false);

                string tmp = nowhereman.Properties.getProperty("CartoMode", MapStyle.Road.ToString());
                if (tmp.Length > 0)
                {
                    CurrentMapStyle = (MapStyle)Enum.Parse(typeof(MapStyle), tmp);
                }

                if (modeDay)
                {
                    MapColor = MapColorScheme.Light;
                }
                else
                {
                    MapColor = MapColorScheme.Dark;
                }


                if (!hasGPS)
                {
                    WaitText1Visibility = Visibility.Collapsed;
                    WaitText2Visibility = Visibility.Collapsed;
                    WaitText3Visibility = Visibility.Collapsed;
                }
                else
                {
                    //speedCanvas3Background = speedCanvas3.Background;
                    //speedCanvas3Opacity = speedCanvas3.Opacity;

                    //if (followCompass)
                    //{
                    //    compass = Compass.GetDefault();
                    //    if (compass != null)
                    //    {
                    //        compass.ReportInterval = 300; //TODO TimeSpan.FromMilliseconds(300);
                    //                                      // TODO compass.c += compass_Calibrate;
                    //        compass.ReadingChanged += Compass_ReadingChanged;
                    //        // TODO compass.Start();
                    //    }
                    //}
                }

                //if (hasMusic)
                //{
                //    MusicService.instance.Playing += Instance_Playing;
                //}


                if (!expertMode)
                {
                    ObservableCollection<PairCodeDesc> listActions = new ObservableCollection<PairCodeDesc>();
                    listActions.Add(new PairCodeDesc("NONE", resourceLoader.GetString("Nothing")));
                    listActions.Add(new PairCodeDesc(Constants.PAUSE, resourceLoader.GetString("PauseAction")));
                    listActions.Add(new PairCodeDesc(Constants.MOVE_NEXT, resourceLoader.GetString("MoveNextAction")));
                    listActions.Add(new PairCodeDesc(Constants.MOVE_PREVIOUS, resourceLoader.GetString("MovePreviousAction")));
                    listActions.Add(new PairCodeDesc(Constants.STOP, resourceLoader.GetString("StopAction")));
                    listActions.Add(new PairCodeDesc(Constants.DISABLE_ENABLE_ADVICES, resourceLoader.GetString("DisableEnableAdvices")));
                    listActions.Add(new PairCodeDesc(Constants.DISABLE_ENABLE_MUSIC, resourceLoader.GetString("DisableEnableMusic")));
                    listActions.Add(new PairCodeDesc(Constants.DISABLE_START_STOP, resourceLoader.GetString("DisableStartAndStop")));
                    listActions.Add(new PairCodeDesc(Constants.ENABLE_START_STOP, resourceLoader.GetString("EnableStartAndStop")));
                    listActions.Add(new PairCodeDesc(Constants.SAY_INFO, resourceLoader.GetString("SayInfo")));

                    Action0 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("Tab" + mode, Constants.MOVE_NEXT), ""))].description;
                    Action1 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("DoubleTab" + mode, Constants.PAUSE), ""))].description;
                    Action2 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("Hold" + mode, "NONE"), ""))].description;

                    Action3 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("VerticalFlick" + mode, Constants.MOVE_NEXT), ""))].description;

                    Action4 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("_VerticalFlick" + mode, Constants.MOVE_PREVIOUS), ""))].description;

                    Action5 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("HorizontalFlick" + mode, Constants.MOVE_NEXT), ""))].description;

                    Action6 = listActions[listActions.IndexOf(new PairCodeDesc(nowhereman.Properties.getProperty("_HorizontalFlick" + mode, Constants.MOVE_PREVIOUS), ""))].description;

                    //    //helpInfo.Width = Window.Current.Bounds.Width;
                    //    //helpInfo.Height = Window.Current.Bounds.Height;

                    HelpInfo = true;
                }
                else
                {
                    start();
                }


            }

            LandmarkLayer = mapElements;

            return Task.CompletedTask;

        }

        private void loadFollowSession(int sessionId)
        {
            var dynamicPolyline = new List<BasicGeoposition>();
            foreach (Mesures current in DataBaseManager.instance.GetMesures(sessionId))
            {
                //    double altitude = current.altitude.HasValue ? current.altitude.Value : 0;
                dynamicPolyline.Add(new BasicGeoposition() {Latitude=current.Latitude, Longitude=current.Longitude });
                //    dynamicPolyline.StrokeColor = System.Windows.Media.Colors.Cyan;
            }
            if (dynamicPolyline.Count > 0)
            mapElements.Add(new PolylineMap() { MapRoute = new Geopath(dynamicPolyline), StrokeThickness = "I".Equals(mode) ? 6 : 4, StrokeColor = Colors.Cyan        });

            //Style sst7 = Application.Current.Resources["PUSHPIN_INFO_STYLE"] as Style;

            foreach (PointInterest tmp in DataBaseManager.instance.GetPoints(idSession)) 
            {
                if (tmp.Message.StartsWith(Constants.INFO_POINTS))
                {
                    if (!Double.IsNaN(tmp.Latitude) && !Double.IsNaN(tmp.Longitude))
                    {
                        mapElements.Add(new TextMap
                        {
                            Location = new Geopoint(new BasicGeoposition() { Latitude = tmp.Latitude, Longitude = tmp.Longitude }),
                            Title = tmp.Message.Substring(Constants.INFO_POINTS.Length),
                            Tag = tmp,
                            StyleText = Application.Current.Resources["PUSHPIN_INFO_STYLE"] as Style,
                            PanelStyleText = Application.Current.Resources["PUSHPIN_PANEL_INFO_STYLE"] as Style
                        });

                        //            Pushpin oneMarker = new Pushpin();
                        //            oneMarker.Tag = tmp;
                        //            oneMarker.Content = tmp.message.Substring(ProgressPage.INFO_POINTS.Length);
                        //            oneMarker.Style = sst7;

                        //            MapOverlay tmp2 = new MapOverlay();
                        //            tmp2.PositionOrigin = new System.Windows.Point(0, 1);
                        //            tmp2.Content = oneMarker;
                        //            tmp2.GeoCoordinate = new GeoCoordinate(tmp.latitude, tmp.longitude, 0);
                        //            markerLayerPoints.Add(tmp2);
                    }
                }
            }
        }

        public async void start()
        {
            requiredAccuracy = nowhereman.Properties.getDoubleProperty("GPSaccuracy" + mode, 5);
            requiredAccuracyTime = nowhereman.Properties.getBoolProperty("timeGPSaccuracy" + mode, false);

            // Windows.System.Display.DisplayRequest.RequestActive();

            // TODO PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            //var pathData = new Paths() { Description = "test", Type =  };

            //DataBaseManager.instance.InsertPath(pathData);
            //idPath = pathData.Id;

            Paths pathData = DataBaseManager.instance.GetPath(idPath);

            /* TODO
            if (nowhereman.Properties.getBoolProperty("intelligenceRotation" + mode, false))
            {
                try
                {

                    accelerometer = new Accelerometer();
                    accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(1000);
                    accelerometer.CurrentValueChanged += accelerometer_CurrentValueChanged;
                    accelerometer.Start();
                    isRotated = true;
                }
                catch (Exception ex)
                {
                    nowhereman.LittleWatson.instance.Error("accelerometer ", ex);
                    nowhereman.Properties.setBoolProperty("intelligenceRotation" + mode, false);
                }
            }
            */
            Status = new ScreenStatus() { _modeDay = modeDay, _followCompass = followCompass, _rotation = isRotated };

            Engine.instance.handlerS += MyStatusChanged;
            Engine.instance.handlerP += MyPositionChanged;
            Engine.instance.handler += ExecuteAction;
            Engine.instance.popup += ExecutePopupActionAsync;
            Engine.instance.handlerA += ExecuteActionA;
            Engine.instance.notifyMovement += engine_notifyMovement;

            idSession = await Engine.instance.start(pathData, hasGPS, requiredAccuracy, requiredAccuracyTime, desc, idSession);

            if (hasGPS)
            {
                //buttonStart.Visibility = Visibility.Collapsed;
                StopVisibility = Visibility.Visible;
                PauseVisibility = Visibility.Collapsed;
                ResumeVisibility = Visibility.Collapsed;
                StartVisibility = Visibility.Visible;
                MapVisibility = Visibility.Visible;
                FollowVisibility = followMap ? Visibility.Collapsed : Visibility.Visible;
                MapTypeVisibility = Visibility.Visible;
                CompassVisibility = Visibility.Visible;

                SpeedCanvasVisibility = Visibility.Collapsed;
                FondoVisibility = Visibility.Collapsed;

                if (hasAdvices)
                {
                    VoiceService.instance.Reader(resourceLoader.GetString("textWaitAccuracy"/* TODO , speechLang*/), false);
                }
            }
            else
            {
                StopVisibility = Visibility.Visible;
                MapVisibility = Visibility.Collapsed;
                FollowVisibility = Visibility.Collapsed;
                MapTypeVisibility = Visibility.Collapsed;

                StatusRect1Visibility = Visibility.Collapsed;
                StatusRect2Visibility = Visibility.Collapsed;
                StatusRect3Visibility = Visibility.Collapsed;
                WaitText1Visibility = Visibility.Collapsed;
                WaitText2Visibility = Visibility.Collapsed;
                WaitText3Visibility = Visibility.Collapsed;

                CompassVisibility = Visibility.Collapsed;

                SpeedCanvasVisibility = Visibility.Visible;
                FondoVisibility = Visibility.Visible;

                PauseVisibility = Visibility.Visible;
                ResumeVisibility = Visibility.Collapsed;
                StartVisibility = Visibility.Collapsed;
                Engine.instance.reallyStart();
            }
        }


        async void MyStatusChanged(Engine eng, string status, double? hacc, double? vacc)
        {
            if (!RunningInBackground)
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                if (dispatcher != null)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        try
                        {
                            if (status != null)
                            {
                                WaitText1 = status;

                                if (status.Equals("KO") || status.Equals("NO DATA"))
                                {
                                    StatusRect1Style = Application.Current.Resources["KO_RE"] as Style;
                                    WaitText1Style = Application.Current.Resources["KO_ST"] as Style;
                                }
                                else if (status.Equals("WAIT"))
                                {
                                    StatusRect1Style = Application.Current.Resources["WAIT_RE"] as Style;
                                    WaitText1Style = Application.Current.Resources["WAIT_ST"] as Style;
                                }
                                else
                                {
                                    StartEnabled = true;

                                    StatusRect1Style = Application.Current.Resources["OK_RE"] as Style;
                                    WaitText1Style = Application.Current.Resources["OK_ST"] as Style;
                                }
                            }

                            if (hacc != null && hacc.HasValue)
                            {
                                double val = hacc.Value;

                                if (val > 99)
                                {
                                    WaitText2 = "99";
                                }
                                else
                                {
                                    WaitText2 = val.ToString("0");
                                }

                                if (val > MAX_ACCURACY)
                                {
                                    StatusRect2Style = Application.Current.Resources["KO_RE"] as Style;
                                    WaitText2Style = Application.Current.Resources["KO_ST"] as Style;
                                    Ellipse.PanelStyleText = Application.Current.Resources["KO_REx"] as Style;
                                }
                                else if (val <= requiredAccuracy)
                                {
                                    StatusRect2Style = Application.Current.Resources["OK_RE"] as Style;
                                    WaitText2Style = Application.Current.Resources["OK_ST"] as Style;
                                    Ellipse.PanelStyleText = Application.Current.Resources["OK_REx"] as Style;
                                }
                                else
                                {
                                    StatusRect2Style = Application.Current.Resources["WAIT_RE"] as Style;
                                    WaitText2Style = Application.Current.Resources["WAIT_ST"] as Style;
                                    Ellipse.PanelStyleText = Application.Current.Resources["WAIT_REx"] as Style;
                                }
                            }

                            if (vacc != null && vacc.HasValue)
                            {
                                double val = vacc.Value;

                                if (val > 99)
                                {
                                    WaitText3 = "99";
                                }
                                else
                                {
                                    WaitText3 = val.ToString("0");
                                }

                                if (val > MAX_ACCURACY)
                                {
                                    StatusRect3Style = Application.Current.Resources["KO_RE"] as Style;
                                    WaitText3Style = Application.Current.Resources["KO_ST"] as Style;
                                }
                                else if (val <= requiredAccuracy)
                                {
                                    StatusRect3Style = Application.Current.Resources["OK_RE"] as Style;
                                    WaitText3Style = Application.Current.Resources["OK_ST"] as Style;
                                }
                                else
                                {
                                    StatusRect3Style = Application.Current.Resources["WAIT_RE"] as Style;
                                    WaitText3Style = Application.Current.Resources["WAIT_ST"] as Style;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            nowhereman.LittleWatson.instance.Error("MyStatusChanged ", ex);
                        }
                    });
                }
            }
        }


        async void MyPositionChanged(Engine eng, TimeSpan _time, double _speed, double _altitude, double _distance, double _ascendent, double _descendent, double distanceFromStart, double distanceToEnd, Geocoordinate e)
        {
            if (e != null)
            {
                lastReceivedPosition = e;
            }

            if (!RunningInBackground)
            {
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;


                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        if (hasGPS)
                        {
                            if (e != null)
                            {
                                //ShowInfo.altitudePoint = e.Point.Position.Altitude;
                                //ShowInfo.Location = new GeoPoint(e.Point.Position);
                                //ShowInfo.d = 
                                /* TODO 
                                if (RouteDirectionsPushPin != null && RouteDirectionsPushPin.Visibility == Visibility.Visible)
                                {
                                    double dist = e.GetDistanceTo(tmpRouteDirectionsPushPin.GeoCoordinate);
                                    string pushpinContent = App.toDistanceString(dist, CultureInfo.CurrentCulture);

                                    if (modePace)
                                    {
                                        if (lastPace > 0)
                                        {
                                            pushpinContent += "\n" + App.toTimeString(TimeSpan.FromMinutes(lastPace * dist / 1000.0).Ticks);
                                        }
                                    }
                                    else
                                    {
                                        if (lastSpeed > 0)
                                        {
                                            pushpinContent += "\n" + App.toTimeString(TimeSpan.FromSeconds(dist / lastSpeed).Ticks);
                                        }
                                    }


                                    this.RouteDirectionsPushPin.Content = pushpinContent.Trim();
                                }
                                */
                                try
                                {
                                    if (Engine.instance.wasStarted)
                                    {
                                        CurrentPoints.Add(e.Point.Position);
                                        // TODO MAX???
                                    }

                                    double accuracy = e.Accuracy;

                                    if (accuracyCur != accuracy)
                                    {
                                        accuracyCur = accuracy;
                                        double metersPerPixels = (Math.Cos(e.Point.Position.Latitude * Math.PI / 180) * 2 * Math.PI * 6378137) / (256 * Math.Pow(2, CurrentMapZoomLevel));
                                        double radius = accuracyCur / metersPerPixels;

                                        Ellipse.Width = radius * 2;
                                        Ellipse.Height = radius * 2;
                                        Ellipse.Location = e.Point;
                                    }

                                    if (followMap)
                                    {
                                        MapCenter = e.Point;
                                    }

                                }
                                catch (Exception ee)
                                {
                                    nowhereman.LittleWatson.instance.Error("MyPositionChanged 1 ", ee);
                                }
                            }

                            try
                            {
                                if (Engine.instance.wasStarted)
                                {
                                    lastKnownPosition = e;
                                    lastDistance = _distance;
                                    lastTime = _time;
                                    lastSpeed = _speed;
                                    lastAltitude = _altitude;
                                }

                                if (modePace)
                                {
                                    if (_distance > 0)
                                    {
                                        lastPace = _time.TotalMinutes / (_distance / 1000.0);
                                        Speed = lastPace;
                                    }
                                }
                                else
                                {
                                    Speed = _speed;
                                }

                                Altitude = _altitude;
                                Distance = _distance;
                                Time = _time;

                                Ascendent = _ascendent;
                                Descendent = _descendent;
                            }
                            catch (Exception ex)
                            {
                                nowhereman.LittleWatson.instance.Error("MyPositionChanged 2 ", ex);
                            }
                        }
                        else
                        {
                            lastTime = _time;
                            Time = _time;
                        }

                        if (stack < 0)
                        {
                            RthymthmRectStyle = Application.Current.Resources["RTHYTHM_KO"] as Style;
                        }
                        else
                        {
                            if (stack > 0)
                            {
                                RthymthmRectStyle = Application.Current.Resources["RTHYTHM_KOK"] as Style;
                            }
                            else
                            {
                                RthymthmRectStyle = Application.Current.Resources["RTHYTHM_OK"] as Style;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        nowhereman.LittleWatson.instance.Error("MyPositionChanged 3 ", ex);
                    }
                });
                refresh = -1;
            }
            else
            {
                try
                {
                    if (Engine.instance.wasStarted)
                    {
                        // IN background 
                        if (hasGPS)
                        {
                            lastKnownPosition = e;
                            lastDistance = _distance;
                            lastTime = _time;
                            lastSpeed = _speed;
                            lastAltitude = _altitude;
                        }
                    }
                    else
                    {
                        lastTime = _time;
                    }

                    //if (hasTile)
                    //{
                    //    long now = DateTime.Now.Ticks;
                    //    if (refresh == -1 || new TimeSpan(now - refresh).TotalSeconds > nowhereman.Properties.getIntProperty("refreshTile", 30))
                    //    {
                    //        updateTile();
                    //        refresh = now;
                    //    }
                    //}

                }
                catch (Exception ex)
                {
                    nowhereman.LittleWatson.instance.Error("MyPositionChanged ", ex);
                }
            }
        }

        async void engine_notifyMovement(Engine engine, int S, int U, int R)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                C0 = (2 * S + U).ToString();
                C1 = (2 * R).ToString();

                if (engine.isIgnored(Constants.DO_PAUSE))
                {
                    C0Style = Application.Current.Resources["PAUSE_ON"] as Style;
                }
                else
                {
                    C0Style = Application.Current.Resources["PAUSE_OFF"] as Style;

                }
                if (engine.isIgnored(Constants.DO_CONTINUE))
                {
                    C1Style = Application.Current.Resources["CONTINUE_ON"] as Style;
                }
                else
                {
                    C1Style = Application.Current.Resources["CONTINUE_OFF"] as Style;
                }
            });
        }

        private void Compass_ReadingChanged(Compass compass, CompassReadingChangedEventArgs args)
        {
            /* TODO
            if (followCompass && compass.IsDataValid)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (map1.Visibility == Visibility.Visible)
                    {
                        if (e.SensorReading.HeadingAccuracy <= 10)
                        {
                            speedCanvas3.Background = speedCanvas3Background;
                            speedCanvas3.Opacity = speedCanvas3Opacity;
                        }
                        else
                        {
                            speedCanvas3.Background = new SolidColorBrush(Colors.Red);
                            speedCanvas3.Opacity = 0.4;
                        }

                        map1.Heading = e.SensorReading.MagneticHeading;
                    }
                });
            }*/
        }

        private void MapLightCommand()
        {
            modeDay = !modeDay;

            Status = new ScreenStatus() { _modeDay = modeDay, _followCompass = followCompass, _rotation = isRotated };
            if (modeDay)
            {
                MapColor = MapColorScheme.Light;
            }
            else
            {
                MapColor = MapColorScheme.Dark;
            }
            nowhereman.Properties.setBoolProperty("modeDay", modeDay);
        }

        private void MapTypeCommand()
        {
            CurrentMapStyle = (MapStyle)(((int)CurrentMapStyle + 1) % 7);
        }

        private void FollowCommand()
        {
            followMap = !followMap;
            if (followMap)
            {
                FollowVisibility = Visibility.Collapsed;
                if (lastReceivedPosition != null)
                {
                    MapCenter = new Geopoint(lastReceivedPosition.Point.Position);
                }

            }
            else
            {
                FollowVisibility = Visibility.Visible;
            }
        }

        private void LocateCommand()
        {
            if (Engine.instance.wasStarted)
            {
                if (MapVisibility == Visibility.Collapsed)
                {
                    MapVisibility = Visibility.Visible;
                    FollowVisibility = followMap ? Visibility.Collapsed : Visibility.Visible;
                    MapTypeVisibility = Visibility.Visible;

                    CompassVisibility = Visibility.Visible;

                    SpeedCanvasVisibility = Visibility.Collapsed;
                    FondoVisibility = Visibility.Collapsed;
                }
                else
                {
                    MapVisibility = Visibility.Collapsed;
                    FollowVisibility = Visibility.Collapsed;
                    MapTypeVisibility = Visibility.Collapsed;
                    CompassVisibility = Visibility.Collapsed;

                    //speedCanvas3.Background = speedCanvas3Background;
                    //speedCanvas3.Opacity = speedCanvas3Opacity;

                    FondoVisibility = Visibility.Visible;
                    //speedCanvas2.Visibility = Visibility.Collapsed;
                    SpeedCanvasVisibility = Visibility.Visible;
                }
            }

        }

        private void PauseCommand(bool force)
        {
            Engine.instance.cancelPopup();

            Engine.instance.pause(force);

        }

        private void ResumeCommand(bool force)
        {
            Engine.instance.cancelPopup();

            Engine.instance.resume(force);

        }

        private async void StopCommand(bool force)
        {
            MessageDialog result = new MessageDialog(resourceLoader.GetString("EndSessionQuestion"), resourceLoader.GetString("attention"));
            result.Commands.Add(new UICommand(resourceLoader.GetString("Cancel"), null, "resume"));
            result.Commands.Add(new UICommand(resourceLoader.GetString("StopRecord"), null, "stop"));
            result.CancelCommandIndex = 0;
            result.DefaultCommandIndex = 0;
            IUICommand selected = await result.ShowAsync();
            if (selected.Id is string && (string)selected.Id == "stop")
            {
                reallyStop(false);
            }

        }

        private async void reallyStop(/* ENGINE eng*/ bool exit)
        {
            VoiceService.instance.Cancel();

            Engine.instance.cancelPopup();
            QuestionPopup = false;

            //stopRadio();
            //try
            //{
            //    if (hasMusic)
            //    {
            //        MusicService.instance.Playing -= Instance_Playing;
            //        MusicService.instance.Pause();
            //        if (lastPoint != null)
            //        {
            //            lastPoint.Rhythm = Engine.instance.toPace(lastTime.Ticks - lastPoint.Time, lastDistance - lastPoint.Distance);
            //            DataBaseManager.instance.InsertPoint(lastPoint);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    nowhereman.LittleWatson.instance.Error("reallyStop music", ex);
            //}

            /* TODO
            if (compass != null)
            {
                compass.Stop();
                //compass.Dispose();
            }

            PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
            */
            Engine.instance._stop(!exit);

            isRotated = false;

            // TODO rotation.Rotation = 0;
            Status = new ScreenStatus() { _modeDay = modeDay, _followCompass = followCompass, _rotation = isRotated };

            //resetTile();

            if (!exit)
            {
                ctlsVisibility = Visibility.Collapsed;

                MapVisibility = Visibility.Collapsed;
                FollowVisibility = Visibility.Collapsed;
                MapTypeVisibility = Visibility.Collapsed;


                CompassVisibility = Visibility.Collapsed;
                //speedCanvas3.Background = speedCanvas3Background;
                //speedCanvas3.Opacity = speedCanvas3Opacity;
                ////speedCanvas2.Visibility = Visibility.Collapsed;
                SpeedCanvasVisibility = Visibility.Visible;
                FondoVisibility = Visibility.Visible;

                //LayoutRoot.RowDefinitions.Clear();

                if (Engine.instance.wasStarted)
                {
                    Sessions session = Engine.instance.getSession();
                    // TODO Say final numbers distance abd time
                    string textFinal = "";
                    if (hasGPS)
                    {
                        string msg = DataBaseManager.instance.analyze(session, mode);

                        textFinal = string.Format(resourceLoader.GetString("endProgress1"/*, speechLang*/), Utils.toReaderTime(lastTime, resourceLoader, VoiceService.instance.SpeechLang),
                            Utils.toReaderDistance(lastDistance, Engine.instance.unit, resourceLoader, VoiceService.instance.SpeechLang), resourceLoader.GetString(mode/*, speechLang*/), Utils.FromNOW(Engine.instance.getSession().DayOfSession));

                        if (msg != null && msg.Length > 0)
                        {
                            textFinal += ".\n" + string.Format(resourceLoader.GetString(msg), resourceLoader.GetString(mode/*, speechLang*/));
                        }
                    }
                    else
                    {
                        textFinal = string.Format(resourceLoader.GetString("endProgress2"/*, speechLang*/), Utils.toReaderTime(lastTime, resourceLoader, VoiceService.instance.SpeechLang), "",
                            resourceLoader.GetString(mode/*, speechLang*/), Utils.FromNOW(Engine.instance.getSession().DayOfSession));
                    }

                    TxtCurrentTrack = textFinal;

                    if (hasAdvices)
                    {
                        VoiceService.instance.Reader(textFinal, false, true);
                    }


                    idSession = session.Id;

                    /* TODO 
                     * if (accelerometer != null)
                    {
                        accelerometer.CurrentValueChanged -= accelerometer_CurrentValueChanged;
                        accelerometer.Stop();
                        accelerometer = null;
                        //accelerometer.Dispose();
                    }

                //    ApplicationBar = new ApplicationBar();
                //    ApplicationBar.IsMenuEnabled = false;

                //    ApplicationBarIconButton button3 = new ApplicationBarIconButton();
                //    button3.IconUri = new Uri("/Images/appbar.overflowdots.png", UriKind.Relative);
                //    button3.Text = AppResources.SeeDetail;
                //    ApplicationBar.Buttons.Add(button3);
                //    button3.Click += new EventHandler(his_Click);

                //    ApplicationBarIconButton button1 = new ApplicationBarIconButton();
                //    button1.IconUri = new Uri("/Images/appbar.share.rest.png", UriKind.Relative);
                //    button1.Text = AppResources.share;
                //    ApplicationBar.Buttons.Add(button1);
                //    button1.Click += new EventHandler(share_Click);
                */

                    if (!expertMode)
                    {
                        MessageDialog result = new MessageDialog(resourceLoader.GetString("NowYoucanShare"), resourceLoader.GetString("Question"));
                        result.Commands.Add(new UICommand(resourceLoader.GetString("Ok"), null, "ok"));
                        result.Commands.Add(new UICommand(resourceLoader.GetString("No"), null, "no"));
                        result.CancelCommandIndex = 0;
                        result.DefaultCommandIndex = 0;
                        IUICommand selected = await result.ShowAsync();
                        if (selected.Id is string && (string)selected.Id == "ok")
                        {
                            var paramsTo = new Dictionary<String, object>();
                            paramsTo.Add("idSession", idSession);
                            paramsTo.Add("mode", mode);

                            NavigationService.ClearHistory();
                            await NavigationService.NavigateAsync(typeof(Views.SharePage), paramsTo);
                        }
                        else
                        {
                            NavigationService.ClearHistory();
                            await NavigationService.NavigateAsync(typeof(Views.MainPage));
                        }
                    }
                    else
                    {
                        NavigationService.ClearHistory();
                        await NavigationService.NavigateAsync(typeof(Views.MainPage));
                    }
                }
                else
                {
                    NavigationService.ClearHistory();
                    await NavigationService.NavigateAsync(typeof(Views.MainPage));
                }
            }
        }

        private void StartCommand(bool force)
        {
            MapVisibility = Visibility.Collapsed;
            FollowVisibility = Visibility.Collapsed;
            MapTypeVisibility = Visibility.Collapsed;
            CompassVisibility = Visibility.Collapsed;
            //speedCanvas2.Visibility = Visibility.Collapsed;
            SpeedCanvasVisibility = Visibility.Visible;
            FondoVisibility = Visibility.Visible;

            /*located.Visibility = Visibility.Visible;
            zoomIn.Visibility = Visibility.Collapsed;
            zoomOut.Visibility = Visibility.Collapsed;
            //buttonStart.Visibility = Visibility.Collapsed;*/
            StopVisibility = Visibility.Visible;
            PauseVisibility = Visibility.Visible;
            ResumeVisibility = Visibility.Collapsed;
            StartVisibility = Visibility.Collapsed;

            Engine.instance.reallyStart();

        }

        private async void ExecutePopupActionAsync(Engine eng, _TriggerAction _action, bool show)
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("ExecutePopupAction " + show);
#endif
            if (show)
            {
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                    //questionPopup.DataContext = LayoutRoot.DataContext;

                    msgPP = _action.toMessage(VoiceService.instance.SpeechLang) + (expertMode ? "" : (". " + resourceLoader.GetString("questionWaitMessage")));

                    if (hasAdvices)
                    {
                        VoiceService.instance.Reader(msgPP, false);
                    }

                //});
                QuestionPopup = true;

            }
            else
            {
                //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //{
                    if (popupTimer != null)
                    {
                        popupTimer.Dispose();
                    }

                    popupTimer = null;

                    VoiceService.instance.Cancel();

                //});
                QuestionPopup = false;
            }

        }

        public void questionPopup_Opened()
        {
            FondoVisibility = Visibility.Collapsed;
            timerN = timeForQuestion;
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("questionPopup_Opened " + timerN + " start");
#endif

            popupTimer = new Timer(async (src) =>
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,  () =>
                {
#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("questionPopup_Opened " + timerN);
#endif
                    if (--timerN <= 0)
                    {
                        QuestionPopup = false;
                    }
                    else
                    {
                        timePP = timerN.ToString();
                    }
                });
            }, new AutoResetEvent(false), 0, 1000);
        }

        public void questionPopup_Closed()
        {
            try
            {
                FondoVisibility = Visibility.Visible;
                if (popupTimer != null)
                {
                    popupTimer.Dispose();
                }
                popupTimer = null;

                Engine.instance.acceptPopup();
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("closed popup", ex);
            }

        }


        private void closePopup()
        {
            try
            {
                Engine.instance.cancelPopup();

            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("tap popup", ex);
            }
        }

        private void ignorePopup()
        {
            try
            {
                Engine.instance.ignorePopup();
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("tap popup", ex);
            }
        }

        private void doAction(string action, string defaultM)
        {
            /*waitText.Text = action;
            return;*/
            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.PAUSE)
            {
                if (Engine.instance.running && !Engine.instance.paused)
                {
                    PauseCommand(false);
                }
                else
                {
                    if (Engine.instance.running && Engine.instance.paused)
                        ResumeCommand(false);
                }
            }

            //if (nowhereman.Properties.getProperty(action, defaultM) == Constants.MOVE_NEXT)
            //{
            //    if (hasMusic)
            //    {
            //        MusicService.instance.SkipNext();
            //    }
            //    else if (hasRadio)
            //    {
            //        nextFreq();
            //    }
            //}
            //if (nowhereman.Properties.getProperty(action, defaultM) == MOVE_PREVIOUS)
            //{
            //    if (hasMusic)
            //    {
            //        MusicService.instance.SkipPrevious();
            //    }
            //    else if (hasRadio)
            //    {
            //        prevFreq();
            //    }
            //}
            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.STOP)
            {
                StopCommand(false);
            }
            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.DISABLE_ENABLE_ADVICES)
            {
                hasAdvices = !hasAdvices;
            }
            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.ENABLE_START_STOP)
            {
                Engine.instance.enableStartAndStop();
            }
            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.DISABLE_START_STOP)
            {
                Engine.instance.disableStartAndStop();
            }
            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.DISABLE_ENABLE_MUSIC)
            {
                hasMusic = !hasMusic; // TODO wrong with radio
                if (hasMusic)
                {
                    MusicService.instance.Play();
                    //TODO
                    //if (_mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
                    //{
                    //    _mediaPlayer.Play();
                    //}
                    //else
                    //{
                    //    changeToList("", true);
                    //}
                }
                else
                {
                    MusicService.instance.Pause();
                }
            }

            if (nowhereman.Properties.getProperty(action, defaultM) == Constants.SAY_INFO)
            {
                TimeSpan time = lastTime;
                double distance = lastDistance;
                string msg = "";
                string _time = Utils.toReaderTime(time, resourceLoader, VoiceService.instance.SpeechLang);
                if (distance > 0)
                {
                    string _distance = Utils.toReaderDistance(distance, Engine.instance.unit, resourceLoader, VoiceService.instance.SpeechLang);
                    msg = resourceLoader.GetString("SayInfoText");

                    msg = string.Format(msg, _time, _distance, resourceLoader.GetString(mode));
                }
                else
                {
                    msg = resourceLoader.GetString("SayInfoText1");

                    msg = string.Format(msg, _time, resourceLoader.GetString(mode));

                }
                VoiceService.instance.Reader(msg, true);
            }
        }


        async void ExecuteAction(Engine eng, _TriggerAction _action)
        {
            try
            {
                TriggerActionDistance __actiond = _action as TriggerActionDistance;
                TriggerActionNumber __action = _action as TriggerActionNumber;
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                if (__action != null && __action.action != null)
                {
                    if (Constants.YOU_GOT_THE_RHYTHM.Equals(__action.action))
                    {
                        playSound(Constants.RHYTHM_SET);
                        return;
                    }
                    else if (Constants.YOU_GO_TO_FAST.Equals(__action.action))
                    {
                        playSound(Constants.TOO_FAST);
                        return;
                    }
                    else if (Constants.YOU_LOST_THE_RHYTHM.Equals(__action.action))
                    {
                        playSound(Constants.COME_ON);
                        return;
                    }
                    else if (Constants.SIGNAL_IS_LOST.Equals(__action.action))
                    {
                        playSound(Constants.SIGNAL_IS_LOST);
                        return;
                    }
                    else if (Constants.YOU_COMPLETE_LOST_THE_RHYTHM.Equals(__action.action))
                    {
                        playSound(Constants.COMPLETE_COME_ON);
                        return;
                    }
                    else if (Constants.SHOW_INFO_YOU_GO_TO_FAST.Equals(__action.action))
                    {
                        ExecuteActionR(eng, __action);
                        return;
                    }
                    else if (Constants.SHOW_INFO_YOU_LOST_THE_RHYTHM.Equals(__action.action))
                    {
                        ExecuteActionR(eng, __action);
                        return;
                    }
                    else if (Constants.DO_PAUSE.Equals(__action.action))
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            PauseCommand(false);
                        });
                        return;
                    }
                    else if (Constants.DO_START.Equals(__action.action))
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            StartCommand(false);
                        });
                        return;
                    }
                    else if (Constants.DO_CONTINUE.Equals(__action.action))
                    {
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            ResumeCommand(false);
                        });
                        return;
                    }
                    else if (Constants.CHANGE_PACE.Equals(__action.action))
                    {
                        eng.changePace((double)_action.param);
                        // continue to say a message           
                    }
                    else if (__action.action.StartsWith(Constants.LOOP_DETECTED))
                    {
                        // TODO
                        playSound(Constants.LOOP_DETECTED);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (__actiond != null && __actiond.action != null)
                {
                    if (Constants.LOOP_DETECTED.Equals(__actiond.action))
                    {
                        //PointInterest lastPoint = new PointInterest();
                        //lastPoint.idSession = idSession;
                        //lastPoint.distance = __actiond.distance;
                        //lastPoint.time = (long)__actiond.param;
                        //lastPoint.latitude = __actiond.position.Latitude;
                        //lastPoint.longitude = __actiond.position.Longitude;
                        //lastPoint.message = ProgressPage.EXTRA_POINTS + __actiond.action;

                        //// To avoid error with engine!
                        //lock (App.DB.lockPoint)
                        //{
                        //    App.DB.PointInterests.InsertOnSubmit(lastPoint);
                        //    App.DB.SubmitChanges();
                        //}

                        //return;
                    }
                }

                _TriggerAction action = (_TriggerAction)_action;
                string msg = action.toMessage(VoiceService.instance.SpeechLang);
                if (msg != null)
                    msg = string.Format(msg, resourceLoader.GetString(mode));

                if (!RunningInBackground)
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Info = msg;
                    });
                }

                if (Constants.CHANGE_MUSIC.Equals(action.action))
                {
                    //    if (hasMusic)
                    //    {
                    //        changeToList(action.param as string, true);
                    //    }
                    //    else if (hasRadio)
                    //    {
                    //        startRadio();
                    //    }
                }
                else
                {
                    string nextSound = changeSong(msg);
                }
            }
            catch (Exception e)
            {
                nowhereman.LittleWatson.instance.Error("ExecuteAction ", e);
            }

        }

        private IDictionary<string, DateTime> lastTimeMessage = new Dictionary<string, DateTime>();
        String soundName = null;
        bool soundIsPlaying = false;

        private bool soundToFast(string a)
        {
            DateTime n = DateTime.Now;

            if (lastTimeMessage.ContainsKey(a))
            {
                DateTime last = lastTimeMessage[a];

                TimeSpan span = TimeSpan.FromTicks(n.Ticks - last.Ticks);

                if (span.TotalSeconds >= 60)
                {
                    lastTimeMessage[a] = n;
                    return true;
                }
            }
            else
            {
                lastTimeMessage.Add(a, n);
            }
            return false;
        }

        private void playSound(string nextSound)
        {
            if (soundIsPlaying)
            {
                return;
            }

            if (soundToFast(nextSound))
            {
                return;
            }

            string _nextSound = changeSong(resourceLoader.GetString(nextSound));

            if (_nextSound != null)
            {
                return;
            }

            //// TODO Test!!!
            if (Constants.COME_ON.Equals(nextSound) || Constants.COMPLETE_COME_ON.Equals(nextSound))
            {
                if (stack >= 0)
                {
                    //        if (hasMusic)
                    //        {
                    //            changeToList(nowhereman.Properties.getProperty("powerList" + mode, resourceLoader.GetString("NONE")), false);
                    //        }
                    stack = -1;
                }
            }
            else
            {
                if (stack <= 0)
                {
                    //        if (hasMusic)
                    //        {
                    //            changeToList(nowhereman.Properties.getProperty("calmList" + mode, resourceLoader.GetString("NONE")), false);
                    //        }
                    stack = 1;
                }
            }

        }

        private string changeSong(string msg)
        {
            if (hasAdvices)
            {
                if ((!soundIsPlaying) && (msg != null || msg != "") && hasAdvices)
                {
                    soundName = msg;
                    try
                    {
                        VoiceService.instance.Reader(msg, true);
                    }
                    catch (Exception e)
                    {
                        nowhereman.LittleWatson.instance.Error("changeSong ", e);
                    }
                    return "NONE";
                }
            }
            return null;
        }

        async void ExecuteActionR(Engine eng, TriggerActionNumber __action)
        {
            try
            {

                if (!RunningInBackground)
                {
                    var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;


                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Style col;
                        col = Application.Current.Resources["OK_STYLE"] as Style;

                        if (Math.Abs(__action.num) >= 2)
                        {
                            if (Constants.SHOW_INFO_YOU_LOST_THE_RHYTHM.Equals(__action.action))
                                col = Application.Current.Resources["KO_STYLE"] as Style;
                            else
                                if (Constants.SHOW_INFO_YOU_GO_TO_FAST.Equals(__action.action))
                                col = Application.Current.Resources["OKK_STYLE"] as Style;
                        }
                        else
                        {
                            if (Constants.SHOW_INFO_YOU_LOST_THE_RHYTHM.Equals(__action.action))
                                col = Application.Current.Resources["KO_STYLE1"] as Style;
                            else
                                if (Constants.SHOW_INFO_YOU_GO_TO_FAST.Equals(__action.action))
                                col = Application.Current.Resources["OKK_STYLE1"] as Style;

                        }

                        SpeedStyle = col;
                    });
                }
            }
            catch (Exception e)
            {
                nowhereman.LittleWatson.instance.Error("ExecuteActionR", e);
            }
        }

        async void ExecuteActionA(Engine eng, string action)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

            if (Constants.PAUSE_ACTION.Equals(action))
            {
                //pauseTile();

                //if (hasMusic)
                //{
                //    MusicService.instance.Pause();
                //}
                //else if (hasRadio)
                //{
                //    pauseRadio();
                //}

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (StartVisibility != Visibility.Visible)
                    {
                        StopVisibility = Visibility.Visible;
                        PauseVisibility = Visibility.Collapsed;
                        ResumeVisibility = Visibility.Visible;
                    }
                });

                if (hasAdvices)
                {
                    VoiceService.instance.Reader(resourceLoader.GetString("PauseWorkoutReader"), false);
                }
            }
            else if (Constants.STOP_ACTION.Equals(action))
            {
                // TODO PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;

                eng._stop(false);

                /* TODO resetTile();*/


                //if (hasMusic)
                //{
                //    MusicService.instance.Playing -= Instance_Playing;
                //    MusicService.instance.Pause();
                //    if (lastPoint != null)
                //    {
                //        lastPoint.Rhythm = Engine.instance.toPace(lastTime.Ticks - lastPoint.Time, lastDistance - lastPoint.Distance);
                //        DataBaseManager.instance.InsertPoint(lastPoint);
                //    }
                //}
                //else
                //{
                //    stopRadio();
                //}


                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ctlsVisibility = Visibility.Collapsed;

                    MapVisibility = Visibility.Collapsed;
                    FollowVisibility = Visibility.Collapsed;
                    MapTypeVisibility = Visibility.Collapsed;

                    CompassVisibility = Visibility.Collapsed;
                    //speedCanvas3.Background = speedCanvas3Background;
                    //speedCanvas3.Opacity = speedCanvas3Opacity;
                    SpeedCanvasVisibility = Visibility.Visible;
                    FondoVisibility = Visibility.Visible;

                    //LayoutRoot.RowDefinitions.Clear();
                });

                if (Engine.instance.wasStarted)
                {
                    Sessions session = eng.getSession();

                    // TODO Say final numbers distance abd time
                    string textFinal = "";
                    if (hasGPS)
                    {
                        string msg = DataBaseManager.instance.analyze(session, mode);

                        textFinal = string.Format(resourceLoader.GetString("endProgress1"), Utils.toReaderTime(lastTime, resourceLoader, VoiceService.instance.SpeechLang),
                            Utils.toReaderDistance(lastDistance, eng.unit, resourceLoader, VoiceService.instance.SpeechLang), resourceLoader.GetString(mode), Utils.FromNOW(eng.getSession().DayOfSession));

                        if (msg != null && msg.Length > 0)
                        {
                            textFinal += ".\n" + string.Format(resourceLoader.GetString(msg), resourceLoader.GetString(mode));
                        }
                    }
                    else
                    {
                        textFinal = string.Format(resourceLoader.GetString("endProgress2"), Utils.toReaderTime(lastTime, resourceLoader, VoiceService.instance.SpeechLang), "",
                            resourceLoader.GetString(mode), Utils.FromNOW(eng.getSession().DayOfSession));
                    }

                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        TxtCurrentTrack = textFinal;
                    });

                    if (hasAdvices)
                    {
                        VoiceService.instance.Reader(textFinal, false);
                    }


                    idSession = session.Id;

                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        /* TODO ApplicationBar = new ApplicationBar();
                        ApplicationBar.IsMenuEnabled = false;

                        ApplicationBarIconButton button3 = new ApplicationBarIconButton();
                        button3.IconUri = new Uri("/Images/appbar.overflowdots.png", UriKind.Relative);
                        button3.Text = resourceLoader.GetString("SeeDetail;
                        ApplicationBar.Buttons.Add(button3);
                        button3.Click += new EventHandler(his_Click);

                        ApplicationBarIconButton button1 = new ApplicationBarIconButton();
                        button1.IconUri = new Uri("/Images/appbar.share.rest.png", UriKind.Relative);
                        button1.Text = resourceLoader.GetString("share;
                        ApplicationBar.Buttons.Add(button1);
                        button1.Click += new EventHandler(share_Click);

                        if (!expertMode)
                        {
                            MessageBoxResult result = MessageBox.Show(resourceLoader.GetString("NowYoucanShare, resourceLoader.GetString("Question, MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                NavigationService.Navigate(new Uri("/SharePage.xaml?id=" + idSession.ToString(), UriKind.Relative));
                            }
                        }*/
                    });
                }
                else
                {
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        /* TODO NavigationService.GoBack();*/
                    });
                }
            }
            else if (Constants.RESUME_ACTION.Equals(action))
            {
                /* TODO
                  resumeTile(); >*/

                //if (hasMusic)
                //{
                //    MusicService.instance.Play();
                //}
                //else if (hasRadio)
                //{
                //    resumeRadio();
                //}
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (StartVisibility != Visibility.Visible)
                    {
                        StopVisibility = Visibility.Visible;
                        PauseVisibility = Visibility.Visible;
                        ResumeVisibility = Visibility.Collapsed;
                    }
                });

                if (hasAdvices)
                {
                    VoiceService.instance.Reader(resourceLoader.GetString("ContinueWorkoutReader"), false);
                }

            }
            else if (Constants.READY_TO_START_ACTION.Equals(action))
            {
                //buttonStop.Visibility = Visibility.Visible;
                //buttonPause.Visibility = Visibility.Collapsed;
                //buttonResume.Visibility = Visibility.Collapsed;
                //buttonReallyStart.Visibility = Visibility.Visible;
                if (hasAdvices)
                {
                    VoiceService.instance.Reader(resourceLoader.GetString("textReadyAccuracy"), false);
                }
            }

        }

        TappedEventHandler _QuestionPopupTapped;
        public TappedEventHandler QuestionPopupTapped
           => _QuestionPopupTapped ?? (_QuestionPopupTapped = new TappedEventHandler((sender,args) =>
           {
               Tap();
           }));
        

        DoubleTappedEventHandler _QuestionPopupDoubleTapped;
        public DoubleTappedEventHandler QuestionPopupDoubleTapped
           => _QuestionPopupDoubleTapped ?? (_QuestionPopupDoubleTapped = new DoubleTappedEventHandler((sender, args) =>
           {
               DoubleTap();
        }));


        HoldingEventHandler _QuestionPopupHolding;
        public HoldingEventHandler QuestionPopupHolding
           => _QuestionPopupHolding ?? (_QuestionPopupHolding = new HoldingEventHandler((sender, args) =>
           {
               HoldTap();
           }
            ));

        internal void Tap()
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("TAP SCREEN");
#endif
            if (QuestionPopup)
            {
                closePopup();
            }
            else
            {
                if (MapVisibility == Visibility.Visible) return;

                if (Engine.instance != null && !Engine.instance.running)
                {
                    //ButtonReallyStart_Click(sender, null);
                }
                else
                {
                    doAction("Tab" + mode, Constants.MOVE_NEXT);
                }
            }
        }

        internal void DoubleTap()
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("DOUBLETAP SCREEN");
#endif
            if (QuestionPopup)
            {

            }
            else
            {
                if (MapVisibility == Visibility.Visible) return;

                if (Engine.instance != null && !Engine.instance.running)
                {
                    //ButtonReallyStart_Click(sender, null);
                }
                else
                {
                    doAction("DoubleTab" + mode, Constants.PAUSE);
                }
            }
        }

        internal void HoldTap()
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("HOLD SCREEN");
#endif
            if (QuestionPopup)
            {

            }
            else
            {
                if (MapVisibility == Visibility.Visible) return;

                if (Engine.instance != null && !Engine.instance.running)
                {
                    //ButtonReallyStart_Click(sender, null);
                }
                else
                {
                    doAction("Hold" + mode, Constants.STOP);
                }
            }
        }

        internal void flick(Orientation Direction, double velocity)
        {
            System.Diagnostics.Debug.WriteLine("FLICK SCREEN " + velocity + " " + Direction.ToString());

            if (QuestionPopup)
            {
                if (Direction == Orientation.Horizontal)
                {
                    ignorePopup();
                }
            }
            else
            {
                if (MapVisibility == Visibility.Visible) return;

                if (Engine.instance != null && !Engine.instance.running)
                {
                    //ButtonReallyStart_Click(sender, null);
                }
                else
                {
                    if (Direction == Orientation.Vertical)
                    {
                        doAction(velocity < 0 ? "-VerticalFlick" + mode : "VerticalFlick" + mode, velocity < 0 ? Constants.MOVE_PREVIOUS : Constants.MOVE_NEXT);
                    }
                    else if (Direction == Orientation.Horizontal)
                    {
                        doAction(velocity < 0 ? "-HorizontalFlick" + mode : "HorizontalFlick" + mode, velocity < 0 ? Constants.MOVE_PREVIOUS : Constants.MOVE_NEXT);
                    }
                }
            }
        }


        public void SpeedCommand(object sender, TappedRoutedEventArgs e)
        {
        }

    }
}
