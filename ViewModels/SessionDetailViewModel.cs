using nowhereman;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Others;
using UniversalKeepTheRhythm.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class SessionDetailViewModel : ViewModelBase
    {
        string _MapServiceToken = "CXLf4gyo8k1gFsG3kyF5~zhvmsbdiZw22us21grWyig~AnJC7VCIbiG4zKbYYwFJg9UKyf2zOwbzdQDNOtZfcDyGXl5NVqOpGnG1Oq3Hu420";
        public string MapServiceToken { get { return _MapServiceToken; } set { Set(ref _MapServiceToken, value); } }

        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        string _Comment = default(string);
        public string Comment { get { return _Comment; } set { Set(ref _Comment, value); } }

        string _NamePath = default(string);
        public string NamePath { get { return _NamePath; } set { Set(ref _NamePath, value); } }

        MapStyle _CurrentMapStyle = MapStyle.Road;
        public MapStyle CurrentMapStyle { get { return _CurrentMapStyle; } set { Set(ref _CurrentMapStyle, value); } }


        Visibility _progressBarInitialVisibility = Visibility.Visible;
        public Visibility progressBarInitialVisibility { get { return _progressBarInitialVisibility; } set { Set(ref _progressBarInitialVisibility, value); } }

        List<object> _LandmarkLayer = default(List<object>);
        public List<object> LandmarkLayer { get { return _LandmarkLayer; } set { Set(ref _LandmarkLayer, value); } }


        MapScene _Scene = default(MapScene);
        public MapScene Scene { get { return _Scene; } set { Set(ref _Scene, value); } }

        Geopoint _MapCenter = default(Geopoint);
        public Geopoint MapCenter { get { return _MapCenter; } set { Set(ref _MapCenter, value); } }
        //public ObservableCollection<MapLayer> LandmarkLayer { get; } = new ObservableCollection<MapLayer>();


        EllipseCanvas _PointToAltitude = default(EllipseCanvas);
        public EllipseCanvas PointToAltitude { get { return _PointToAltitude; } set { Set(ref _PointToAltitude, value); } }

        SizeChangedEventHandler _SizeChanged;
        public SizeChangedEventHandler SizeChanged
           => _SizeChanged ?? (_SizeChanged = new SizeChangedEventHandler((sender, args) =>
           {
               altitudeCanvasHeight = args.NewSize.Height;
               altitudeCanvasWidth = args.NewSize.Width;
               calculateCanvas();
           }));

        EllipseCanvas _PointToSpeed = default(EllipseCanvas);
        public EllipseCanvas PointToSpeed { get { return _PointToSpeed; } set { Set(ref _PointToSpeed, value); } }

        private int idSession;
        private Sessions sessionObj;
        private Paths pathObj;

        List<EnrichedMesure> MesuresList = new List<EnrichedMesure>();

        //        List<double> listRhythm = new List<double>();
        //        List<double> listDistance = new List<double>();
        //        List<double> listAltitude = new List<double>();
        //        List<double> listSpeed = new List<double>();
        //        List<double> listPace = new List<double>();
        //#if(PACE)
        //        List<Point> graphPointsCollectionPaceTotal = new List<Point>();
        //#endif
        //        List<long> listTime = new List<long>();
        //List<int> listCurrentX = new List<int>();

        bool modePace;
        double MIN_DELTA_ALTITUDE = 5;


        //List<BasicGeoposition> listPoint = new List<BasicGeoposition>();
        //List<PointInterest> music = new List<PointInterest>();

        //double HEIGHT = 300;
        //double WIDTH = 870;

        int POINT_WIDTH = 5;

        int totalData = 0, ignore = 0;

        List<MapElement> markerLayer = new List<MapElement>();
        MapLayer markerLayerMusic = null;


        int visibleMode = 0;

        double margin;
        string mode = "R";
        int SECONDS_TO_DISTANCE = 5;
        double graphHeight;
        double unitDist = 10;

        double maxDistance = 1000;
        double MinSpeed = Double.MaxValue, MaxSpeed = Double.MinValue, MinPace = Double.MaxValue, MaxPace = Double.MinValue, MinAltitude = Double.MaxValue, MaxAltitude = Double.MinValue, SumPace = 0, AvgPace = 0;
        double factPace;

        ShowInfoCanvas _ShowInfo = new ShowInfoCanvas();
        public ShowInfoCanvas ShowInfo { get { return _ShowInfo; } set { Set(ref _ShowInfo, value); } }

        double _altitudeCanvasWidth = default(double);
        public double altitudeCanvasWidth { get { return _altitudeCanvasWidth; } set { Set(ref _altitudeCanvasWidth, value); } }

        double _altitudeCanvasHeight = default(double);
        public double altitudeCanvasHeight { get { return _altitudeCanvasHeight; } set { Set(ref _altitudeCanvasHeight, value); } }

        double _fondoWidth = default(double);
        public double fondoWidth { get { return _fondoWidth; } set { Set(ref _fondoWidth, value); } }

        double _fondoHeight = default(double);
        public double fondoHeight { get { return _fondoHeight; } set { Set(ref _fondoHeight, value); } }


        Visibility _ctlLapsVisibility = Visibility.Collapsed;
        public Visibility ctlLapsVisibility { get { return _ctlLapsVisibility; } set { Set(ref _ctlLapsVisibility, value); } }

        string _summaryData = default(string);
        public string summaryData { get { return _summaryData; } set { Set(ref _summaryData, value); } }

        TimeSpan _Duration = default(TimeSpan);
        public TimeSpan Duration { get { return _Duration; } set { Set(ref _Duration, value); } }

        double _Pace = default(double);
        public double Pace { get { return _Pace; } set { Set(ref _Pace, value); } }

        double _PlanedPace = default(double);
        public double PlanedPace { get { return _PlanedPace; } set { Set(ref _PlanedPace, value); } }

        double _Distance = default(double);
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }

        double _Ascendent = default(double);
        public double Ascendent { get { return _Ascendent; } set { Set(ref _Ascendent, value); } }

        double _Descendent = default(double);
        public double Descendent { get { return _Descendent; } set { Set(ref _Descendent, value); } }

        double _SpeedMin = default(double);
        public double SpeedMin { get { return _SpeedMin; } set { Set(ref _SpeedMin, value); } }
        double _SpeedMax = default(double);
        public double SpeedMax { get { return _SpeedMax; } set { Set(ref _SpeedMax, value); } }

        double _AltitudeMin = default(double);
        public double AltitudeMin { get { return _AltitudeMin; } set { Set(ref _AltitudeMin, value); } }
        double _AltitudeMax = default(double);
        public double AltitudeMax { get { return _AltitudeMax; } set { Set(ref _AltitudeMax, value); } }

        double _SpeedAvg = default(double);
        public double SpeedAvg { get { return _SpeedAvg; } set { Set(ref _SpeedAvg, value); } }

        double _AltitudeAvg = default(double);
        public double AltitudeAvg { get { return _AltitudeAvg; } set { Set(ref _AltitudeAvg, value); } }


        DelegateCommand _CenterAction;
        public DelegateCommand CenterAction
           => _CenterAction ?? (_CenterAction = new DelegateCommand(() => fixView(), () => true));


        DelegateCommand _ExpandAction;
        public DelegateCommand ExpandAction
           => _ExpandAction ?? (_ExpandAction = new DelegateCommand(() =>
           {
               throw new NotImplementedException();
           }, () => true));


        DelegateCommand _MapTypeAction;
        public DelegateCommand MapTypeAction
           => _MapTypeAction ?? (_MapTypeAction = new DelegateCommand(() => MapTypeCommand(), () => true));


        DelegateCommand _NewWorkoutCommand;
        public DelegateCommand NewWorkoutCommand
           => _NewWorkoutCommand ?? (_NewWorkoutCommand = new DelegateCommand(async () =>
           {
               var paramsTo = new Dictionary<String, object>();
               paramsTo.Add("sessionId", sessionObj.Id);
               paramsTo.Add("id", pathObj.Id);
               paramsTo.Add("mode", mode);

               await NavigationService.NavigateAsync(typeof(Views.NewSessionPage), paramsTo);
           }, () => true));


        DelegateCommand _ShareCommand;
        public DelegateCommand ShareCommand
           => _ShareCommand ?? (_ShareCommand = new DelegateCommand(async () =>
           {
               DataTransferManager.ShowShareUI();
           }, () => true));


        DelegateCommand _ContinueWorkoutCommand;
        public DelegateCommand ContinueWorkoutCommand
           => _ContinueWorkoutCommand ?? (_ContinueWorkoutCommand = new DelegateCommand(async () =>
           {
               var paramsTo = new Dictionary<String, object>();
               paramsTo.Add("idSession", sessionObj.Id);
               paramsTo.Add("id", pathObj.Id);
               paramsTo.Add("mode", mode);

               await NavigationService.NavigateAsync(typeof(Views.NewSessionPage), paramsTo);
           }, () => true));

        TappedEventHandler _Canvas_Tapped;
        public TappedEventHandler Canvas_Tapped
           => _Canvas_Tapped ?? (_Canvas_Tapped = new TappedEventHandler((sender, args) =>
           {
               double newLeft = /*WIDTH -*/ args.GetPosition((UIElement)sender).X;
               showAt(newLeft);
           }));

        TappedEventHandler _Loop_Tapped;
        public TappedEventHandler Loop_Tapped
           => _Loop_Tapped ?? (_Loop_Tapped = new TappedEventHandler((sender, args) =>
           {
               var tmp = (FrameworkElement)sender;
               if (tmp != null)
                   SeeLoop((int)tmp.Tag);
           }));

        PointerEventHandler _Canvas_PointerMoved;
        public PointerEventHandler Canvas_PointerMoved
           => _Canvas_PointerMoved ?? (_Canvas_PointerMoved = new PointerEventHandler((sender, args) =>
           {
               double newLeft = /*WIDTH -*/ args.GetCurrentPoint((UIElement)sender).Position.X;

               showAt(newLeft);
           }));

        DelegateCommand _ShowInfoCommand;
        public DelegateCommand ShowInfoCommand
           => _ShowInfoCommand ?? (_ShowInfoCommand = new DelegateCommand(() =>
           {
               throw new NotImplementedException();
           }, () => true));


        DelegateCommand _DeleteCommand;
        public DelegateCommand DeleteCommand
           => _DeleteCommand ?? (_DeleteCommand = new DelegateCommand(() =>
           {
               DeleteAction();
           }, () => true));



        DelegateCommand _FixCommand;
        public DelegateCommand FixCommand
           => _FixCommand ?? (_FixCommand = new DelegateCommand(() =>
           {
               if (FixAction())
               {
                   loadData();
               }
           }, () => true));


        DelegateCommand _CompressCommand;
        public DelegateCommand CompressCommand
           => _CompressCommand ?? (_CompressCommand = new DelegateCommand(() =>
           {
               if (CompressAction())
               {
                   loadData();
               }
           }, () => true));

        Visibility _ctlDetailVisibility = Visibility.Visible;
        public Visibility ctlDetailVisibility { get { return _ctlDetailVisibility; } set { Set(ref _ctlDetailVisibility, value); } }

        Visibility _ctlMapVisibility = Visibility.Collapsed;
        public Visibility ctlMapVisibility { get { return _ctlMapVisibility; } set { Set(ref _ctlMapVisibility, value); } }
        Visibility _ctlPerfileVisibility = Visibility.Collapsed;
        public Visibility ctlPerfileVisibility { get { return _ctlPerfileVisibility; } set { Set(ref _ctlPerfileVisibility, value); } }


        List<ObjectCanvas> _altitudeCanvasText = default(List<ObjectCanvas>);
        public List<ObjectCanvas> altitudeCanvasText { get { return _altitudeCanvasText; } set { Set(ref _altitudeCanvasText, value); } }

        LapDetail _LapDetailSelected = default(LapDetail);
        public LapDetail LapDetailSelected { get { return _LapDetailSelected; } set { Set(ref _LapDetailSelected, value); } }

        List<LapDetail> _lapsDetail = default(List<LapDetail>);
        public List<LapDetail> LapsDetail { get { return _lapsDetail; } set { Set(ref _lapsDetail, value); } }

        ObservableCollection<PairCodeDesc> _modeSelectorList = default(ObservableCollection<PairCodeDesc>);
        public ObservableCollection<PairCodeDesc> modeSelectorList { get { return _modeSelectorList; } set { Set(ref _modeSelectorList, value); } }

        PairCodeDesc _modeSelector = default(PairCodeDesc);
        public PairCodeDesc modeSelector { get { return _modeSelector; } set { Set(ref _modeSelector, value); } }

        string _dateSession = default(string);
        public string dateSession { get { return _dateSession; } set { Set(ref _dateSession, value); } }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= ShareViewModel_DataRequested;

            pathObj.Description = NamePath;
            sessionObj.Comment = Comment;
            pathObj.Type = modeSelector.code;

            // TODO

            DataBaseManager.instance.UpdatePath(pathObj);
            DataBaseManager.instance.UpdateSession(sessionObj);

            return Task.CompletedTask;
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {
            DataTransferManager.GetForCurrentView().DataRequested += ShareViewModel_DataRequested;

            string tmp = nowhereman.Properties.getProperty("CartoMode", MapStyle.Road.ToString());
                if (tmp.Length > 0)
                {
                    CurrentMapStyle = (MapStyle)Enum.Parse(typeof(MapStyle), tmp);
                }

                idSession = (int)parameter;

                sessionObj = DataBaseManager.instance.GetSession(idSession);
                pathObj = DataBaseManager.instance.GetPath(sessionObj.IdPath);

                mode = pathObj.Type;

                ObservableCollection<PairCodeDesc> listUnits = new ObservableCollection<PairCodeDesc>();
                listUnits.Add(new PairCodeDesc("R", resourceLoader.GetString("R")));
                listUnits.Add(new PairCodeDesc("C", resourceLoader.GetString("C")));
                listUnits.Add(new PairCodeDesc("W", resourceLoader.GetString("W")));
                listUnits.Add(new PairCodeDesc("I", resourceLoader.GetString("I")));

                modePace = nowhereman.Properties.getBoolProperty("modePace" + mode, true);

                modeSelectorList = listUnits;
                modeSelector = listUnits[listUnits.IndexOf(new PairCodeDesc(mode, ""))];

                //otherTitle.Text = Literals.ResourceManager.GetString(mode);
                dateSession = Others.Utils.FromNOWLong(sessionObj.DayOfSession);

                ////ctlMusic.Visibility = Visibility.Collapsed;

                NamePath = pathObj.Description;
                Comment = sessionObj.Comment;

                _loadData();

            return Task.CompletedTask;
        }


        private async void ShareViewModel_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequestDeferral deferral = args.Request.GetDeferral();
            try
            {
                var request = args.Request;

                request.Data.Properties.Title = pathObj.Description + " @ " + sessionObj.DayOfSession;
                request.Data.Properties.Description = pathObj.Description + " @ " + sessionObj.DayOfSession;

                List<IStorageItem> l = new List<IStorageItem>();
                String unit = nowhereman.Properties.getProperty("units", "m");

                string format = nowhereman.Properties.getProperty("shareFormat", "kml");
                StorageFile file = format == "gpx" ? await UniversalKeepTheRhythm.Others.ExportImport.exportGpx(idSession, "route.gpx", unit, resourceLoader)
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
        private void _loadData()
        {
            if (sessionObj.EndLat.HasValue && sessionObj.EndLon.HasValue && sessionObj.Distance.HasValue && sessionObj.StartLat.HasValue && sessionObj.StartLon.HasValue)
            {
                loadData();
            }
            else
            {
                if (sessionObj.Duration.HasValue)
                {
                    Duration = new TimeSpan(sessionObj.Duration.Value);
                }
                if (sessionObj.Distance.HasValue)
                {
                    Distance = sessionObj.Distance.Value;
                }


                // TODO
                //foreach (PointInterest tmp in App.DB.getPoints(idSession)) // sessionObj.PointInterests) //
                //{
                //    if (ProgressPage.HIGHER.Equals(tmp.message) || ProgressPage.LOWER.Equals(tmp.message))
                //    {
                //        continue;
                //    }

                //    music.Add(tmp);
                //}

                //listMusic.ItemsSource = music;
                //// No data
                //ctlMap.Visibility = Visibility.Collapsed;
                //ctlPerfile.Visibility = Visibility.Collapsed;
                //ctlDetail.Visibility = Visibility.Collapsed;

                progressBarInitialVisibility = Visibility.Collapsed;
            }
        }

        double verticalUnit = 0;
        double horizontalUnit = 0;
        int labelsHeight = 80;
        double spaceSpeed;
        double factSpeed;
        double medSpeed;
        List<Point> graphPointsCollectionAltitude, graphPointsCollectionSpeed;

        private void calculateCanvas()
        {
            if (altitudeCanvasHeight <= 0 || altitudeCanvasWidth <= 0 || double.IsNaN(altitudeCanvasWidth) || double.IsNaN(altitudeCanvasHeight)) return;
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("START DRAW");
#endif
            try
            {
                graphPointsCollectionAltitude = new List<Point>();
                graphPointsCollectionSpeed = new List<Point>();

                var textsOnCanvas = new List<ObjectCanvas>();

                String unit = nowhereman.Properties.getProperty("units", "m");

                verticalUnit = 0;
                horizontalUnit = 0;

                MinAltitude = 0;

                horizontalUnit = maxDistance / altitudeCanvasWidth;
                graphHeight = 5 * altitudeCanvasHeight / 6 - labelsHeight;

                if ((MaxAltitude - MinAltitude) / horizontalUnit > graphHeight)
                {
                    verticalUnit = (MaxAltitude - MinAltitude) / graphHeight;
                }
                else
                {
                    verticalUnit = horizontalUnit;
                }

                spaceSpeed = altitudeCanvasHeight - 5 * altitudeCanvasHeight / 6;
                factSpeed = (MaxSpeed - MinSpeed) / spaceSpeed;
                medSpeed = (MaxSpeed + MinSpeed) / 2;

                factPace = (MaxPace - 0/*MinPace*/) / spaceSpeed;
                Color prevcol = "I".Equals(mode) ? Colors.Green : Others.Utils.getColor(MesuresList[0].Rhythm, margin);

                PointCollection _graphPointsCollectionSpeed2 = new PointCollection();

                int previousX = -1, currentX = 0;
                int nA = 0, nS = 0, nR = 0;
                double vA = 0.0, vS = 0.0, vR = 0.0;

                foreach (var tmp in MesuresList)
                {
                    currentX = (int)Math.Round(tmp.Distance / horizontalUnit, 0);

                    if (previousX != -1 && currentX != previousX)
                    {
                        if (nA > 0)
                            vA /= nA;
                        if (nS > 0)
                            vS /= nS;
                        if (nR > 0)
                            vR /= nR;

                        graphPointsCollectionAltitude.Add(new Point(previousX, spaceSpeed + graphHeight - ((vA - MinAltitude) / verticalUnit)));

                        if (factSpeed != 0)
                        {
                            Color col = Others.Utils.getColor(vR, margin);

                            graphPointsCollectionSpeed.Add(new Point(previousX, spaceSpeed - (vS - MinSpeed) / factSpeed));
                            if (prevcol != col)
                            {
                                _graphPointsCollectionSpeed2.Add(new Point(previousX, spaceSpeed - (vS - MinSpeed) / factSpeed));

                                textsOnCanvas.Add(new PolylineCanvas()
                                {
                                    Top = 0,
                                    Left = 0,
                                    Points = _graphPointsCollectionSpeed2,
                                    StyleText = Application.Current.Resources["graphLineFirst"] as Style,
                                    Brush = new SolidColorBrush(prevcol)
                                });

                                _graphPointsCollectionSpeed2 = new PointCollection();
                                _graphPointsCollectionSpeed2.Add(new Point(previousX, spaceSpeed - (vS - MinSpeed) / factSpeed));
                            }
                            else
                            {
                                _graphPointsCollectionSpeed2.Add(new Point(previousX, spaceSpeed - (vS - MinSpeed) / factSpeed));
                            }

                            prevcol = col;
                        }
                        nA = 0; nS = 0; nR = 0;
                        vA = 0.0; vS = 0.0; vR = 0.0;
                    }

                    if (!double.IsNaN(tmp.Altitude))
                    {
                        vA += tmp.Altitude;
                        nA++;
                    }
                    if (!double.IsNaN(tmp.Speed))
                    {
                        vS += tmp.Speed;
                        nS++;
                    }
                    if (!double.IsNaN(tmp.Rhythm))
                    {
                        vR += tmp.Rhythm;
                        nR++;
                    }

                    previousX = currentX;

                }

                if (nA > 0 || nS > 0 || nR > 0)
                {
                    if (nA > 0)
                        vA /= nA;
                    if (nS > 0)
                        vS /= nS;
                    if (nR > 0)
                        vR /= nR;

                    graphPointsCollectionAltitude.Add(new Point(currentX, spaceSpeed + graphHeight - ((vA - MinAltitude) / verticalUnit)));

                    if (factSpeed != 0)
                    {
                        Color col = Others.Utils.getColor(vR, margin);

                        graphPointsCollectionSpeed.Add(new Point(currentX, spaceSpeed - (vS - MinSpeed) / factSpeed));
                        if (prevcol != col)
                        {
                            _graphPointsCollectionSpeed2.Add(new Point(currentX, spaceSpeed - (vS - MinSpeed) / factSpeed));

                            textsOnCanvas.Add(new PolylineCanvas()
                            {
                                Top = 0,
                                Left = 0,
                                Points = _graphPointsCollectionSpeed2,
                                StyleText = Application.Current.Resources["graphLineFirst"] as Style,
                                Brush = new SolidColorBrush(prevcol)
                            });

                            _graphPointsCollectionSpeed2 = new PointCollection();
                            _graphPointsCollectionSpeed2.Add(new Point(currentX, spaceSpeed - (vS - MinSpeed) / factSpeed));
                        }
                        else
                        {
                            _graphPointsCollectionSpeed2.Add(new Point(currentX, spaceSpeed - (vS - MinSpeed) / factSpeed));
                        }

                        prevcol = col;
                    }
                }

                if (factSpeed != 0)
                {
                    textsOnCanvas.Add(new PolylineCanvas()
                    {
                        Top = 0,
                        Left = 0,
                        Points = _graphPointsCollectionSpeed2,
                        StyleText = Application.Current.Resources["graphLineFirst"] as Style,
                        Brush = new SolidColorBrush(prevcol)
                    });
                }

                if (sessionObj.Duration.HasValue)
                {
                    if (LapsDetail.Count() > 1)
                    {
                        PointCollection ser = new PointCollection();
                        double prevValue = 0.0;

                        textsOnCanvas.Add(new PolygonCanvas() { Top = 0, Left = 0, Points = ser, StyleText = Application.Current.Resources["PACE_POLY_STYLE"] as Style });

                        for (int i = 0, I = LapsDetail.Count; i < I; i++)
                        {
                            int newDist = (int)Math.Round(LapsDetail[i].Distance / horizontalUnit, 0);

                            //int T = (int)Math.Round((double)(newDist) * gridDistanceUnit, 0);

                            ser.Add(new Point(prevValue + 1, spaceSpeed + ((0 - 0/*MinPace*/) / factPace)));
                            ser.Add(new Point(prevValue + 1, spaceSpeed + ((LapsDetail[i].Pace - 0/*MinPace*/) / factPace)));
                            ser.Add(new Point(newDist - 2, spaceSpeed + ((LapsDetail[i].Pace - 0/*MinPace*/) / factPace)));
                            ser.Add(new Point(newDist - 2, spaceSpeed + ((0 - 0/*MinPace*/) / factPace)));

                            var tendency = LapsDetail[i].Tendency;
                            textsOnCanvas.Add(new TextCanvas() { Top = (double)(spaceSpeed - 5 + ((LapsDetail[i].Pace - 0/*MinPace*/) / factPace)), Left = (double)(prevValue), Text = Others.Utils.toPaceStringShort(LapsDetail[i].Pace, unit, CultureInfo.CurrentCulture), StyleText = Application.Current.Resources[tendency == 0 ? "PACE_TXT_STYLE" : tendency > 0 ? "PACE_TXT_STYLE_N" : "PACE_TXT_STYLE_P"] as Style, Rotation = -90, Center = new Point(0, -1) });
                            prevValue = newDist;
                        }

                        ser.Add(new Point(0, spaceSpeed + ((0 - 0/*MinPace*/) / factPace)));
                    }
                }


                double distance = 40.0 * horizontalUnit;
                int digits = (int)Math.Round(Math.Log10(distance), 0.0);
                if (digits <= 0) digits = 1;
                double mask = Math.Pow(10.0, digits);
                distance = Math.Ceiling(distance / mask) * mask;
                int points = (int)Math.Round(distance / horizontalUnit, 0.0);

                for (int currentY = 0; currentY < graphHeight; currentY += 40)
                {
                    textsOnCanvas.Add(new LineCanvas() { Left = 0, Top = spaceSpeed + graphHeight - currentY, X2 = altitudeCanvasWidth, Y2 = 0, StyleText = Application.Current.Resources["ALTITUDE_LINE_STYLE"] as Style });
                }

                for (currentX = 0; currentX < altitudeCanvasWidth; currentX += points)
                {
                    double dis = currentX * horizontalUnit;
                    textsOnCanvas.Add(new TextCanvas() { Left = currentX, Top = graphHeight + spaceSpeed, Text = Others.Utils.toDistanceString(dis, unit, CultureInfo.CurrentCulture).Replace(" ", ""), StyleText = Application.Current.Resources["DISTANCE_TEXT_STYLE"] as Style, Rotation = 20 });
                    textsOnCanvas.Add(new LineCanvas() { Left = currentX, Top = spaceSpeed, X2 = 0, Y2 = graphHeight, StyleText = Application.Current.Resources["ALTITUDE_LINE_STYLE"] as Style });
                }

#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 11");
#endif

                var _graphPointsCollectionAltitude = new PointCollection();
                var _backgroundPointsCollectionAltitude = new PointCollection();

                foreach (var tmp in graphPointsCollectionAltitude)
                {
                    _graphPointsCollectionAltitude.Add(tmp);
                    _backgroundPointsCollectionAltitude.Add(tmp);
                }

                textsOnCanvas.Add(new PolylineCanvas() { Top = 0, Left = 0, Points = _graphPointsCollectionAltitude, StyleText = Application.Current.Resources["graphLineFirst"] as Style });

                Point bottomRightPoint = new Point(altitudeCanvasWidth, altitudeCanvasHeight - labelsHeight);
                Point bottomLeftPoint = new Point(0, altitudeCanvasHeight - labelsHeight);

                _backgroundPointsCollectionAltitude.Add(bottomRightPoint);
                _backgroundPointsCollectionAltitude.Add(bottomLeftPoint);


                textsOnCanvas.Add(new PolygonCanvas() { Top = 0, Left = 0, Points = _backgroundPointsCollectionAltitude, StyleText = Application.Current.Resources["graphBackgroundFirst"] as Style });


                visibleMode = 0;
                if (totalData > 1)
                {
                    textsOnCanvas.Add(ShowInfo);
                    textsOnCanvas.Add(PointToAltitude = new EllipseCanvas() { Width = 10, Height = 10, StyleText = Application.Current.Resources["graphAltitudePoint"] as Style });
                    textsOnCanvas.Add(PointToSpeed = new EllipseCanvas() { Width = 10, Height = 10, StyleText = Application.Current.Resources["graphSpeedPoint"] as Style });

                    altitudeCanvasText = textsOnCanvas;
#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("show " + totalData + " " + altitudeCanvasWidth + "-" + altitudeCanvasHeight);
#endif
                }
            }
            catch (Exception ex)
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                nowhereman.LittleWatson.instance.Error("draw", ex);
            }
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("END DRAW");
#endif
        }

        private void loadData()
        {
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("START LOAD DATA");
#endif
            try
            {

                String unit = nowhereman.Properties.getProperty("units", "m");

                //double factVert = 1;
                //minAltitude = sessionObj.MinAltitude.Value;
                //maxAltitude = sessionObj.MaxAltitude.Value;

                bool hasLoop = nowhereman.Properties.getBoolProperty("hasLoops" + mode, false);
                double LOOP = nowhereman.Properties.getDoubleProperty("loopDistance" + mode, 1000.0);

                maxDistance = (Math.Ceiling(sessionObj.Distance.Value / unitDist)) * unitDist;

                margin = nowhereman.Properties.getDoubleProperty("IntervalForLimit" + mode, 0.05);
                SECONDS_TO_DISTANCE = nowhereman.Properties.getIntProperty("secondsToDistance" + mode, 5);

                if (sessionObj.Duration.HasValue)
                {
                    // Import has duration on sessionObj.
                    Duration = new TimeSpan(sessionObj.Duration.Value);
                }
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 1");
#endif
                if (sessionObj.Distance != null)
                {
                    Ascendent = sessionObj.Ascendent.HasValue ? sessionObj.Ascendent.Value : double.NaN;
                    Descendent = sessionObj.Descendent.HasValue ? sessionObj.Descendent.Value : double.NaN;

                    SpeedMin = sessionObj.MinSpeed.HasValue ? sessionObj.MinSpeed.Value : double.NaN;
                    SpeedMax = sessionObj.MaxSpeed.HasValue ? sessionObj.MaxSpeed.Value : double.NaN;

                    SpeedAvg = sessionObj.Duration.HasValue ? sessionObj.Distance.Value / TimeSpan.FromTicks(sessionObj.Duration.Value).TotalSeconds : double.NaN;

                    AltitudeMin = sessionObj.MinAltitude.HasValue ? sessionObj.MinAltitude.Value : double.NaN;
                    AltitudeMax = sessionObj.MaxAltitude.HasValue ? sessionObj.MaxAltitude.Value : double.NaN;
                    AltitudeAvg = sessionObj.AvgAltitude.Value;

                    Distance = sessionObj.Distance.Value;

                    if (sessionObj.Duration.HasValue)
                    {
                        Pace = sessionObj.AvgPace.HasValue ? sessionObj.AvgPace.Value : double.NaN;
                    }
                    else if ("I".Equals(mode))
                    {
                        if (sessionObj.Duration.HasValue)
                        {
                            Pace = Others.Utils.toPaceSecMet((long)TimeSpan.FromTicks(sessionObj.Duration.Value).TotalSeconds, sessionObj.Distance.Value);
                        }
                    }
                }
                else
                {
                    ctlDetailVisibility = Visibility.Collapsed;
                    Pace = double.NaN;
                }
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 2");
#endif

                if (sessionObj.Duration.HasValue)
                {
                    if (sessionObj.ObjTime.HasValue && sessionObj.ObjDistance.HasValue)
                    {
                        PlanedPace = Others.Utils.toPaceSecMet(sessionObj.ObjTime.Value, sessionObj.ObjDistance.Value);
                    }
                    else
                    {
                        PlanedPace = double.NaN;
                    }
                }
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 3");
#endif


                double curDistance = 0;
                int preDist = -1;
                var sessions2 = DataBaseManager.instance.GetMesures(idSession); // sessionObj.Mesures; // 
                long firstTime = long.MinValue;
                EnrichedMesure PreviousMesure = null;
                long previousLoopId = 0;
                double previousLoopDistance = 0;
                bool isLoopDetected = false;
                bool isAutomaticLoopDetected = false;
                double vS = 0, vA = 0;
                double sumS = 0, sumA = 0, Na = 0, Ns = 0;

#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step mesures");
#endif

                var mesuresList = new List<EnrichedMesure>();
                var lapsDetail = new List<LapDetail>();

                foreach (Mesures current in sessions2)
                {
                    var NewMesure = new EnrichedMesure(current);

                    totalData++;

                    if (PreviousMesure == null)
                    {
                        firstTime = current.Id;
                        previousLoopId = current.Id;
                        previousLoopDistance = 0.0;
                    }
                    else
                    {
                        double dist = PreviousMesure.Position.GetDistanceTo(NewMesure.Position);

                        curDistance += dist;

                        if (hasLoop)
                        {
                            bool _isLoopDetected = Constants.LOOP_DETECTED.Equals(current.Action);
                            bool _isAutomaticLoopDetected = Math.Floor((curDistance - dist) / LOOP) < Math.Floor(curDistance / LOOP);

                            if (_isLoopDetected || (!isLoopDetected && _isAutomaticLoopDetected))
                            {
                                bool wasResetLoop = false;
                                if (_isLoopDetected)
                                {
                                    if (!isLoopDetected)
                                    {
                                        // change mode!
                                        if (isAutomaticLoopDetected)
                                        {
                                            lapsDetail.Clear();
                                            previousLoopDistance = 0;
                                            previousLoopId = firstTime;

                                            wasResetLoop = true;
                                        }
                                        isLoopDetected = true;
                                    }
                                }
                                else if (_isAutomaticLoopDetected)
                                {
                                    isAutomaticLoopDetected = true;
                                }

                                double d = curDistance - previousLoopDistance;
                                long t = current.Id - previousLoopId;

                                LapDetail tmpLap = new LapDetail() { Position = NewMesure.Position, Distance = curDistance, Pace = Others.Utils.toPace(t, d), Time = current.Id - firstTime/*, Tendency = tendency*/ };
                                lapsDetail.Add(tmpLap);

                                previousLoopDistance = curDistance;
                                previousLoopId = current.Id;

                                if (wasResetLoop)
                                {
                                    //    foreach (var tmp in mesureList)
                                    //    {
                                    //        tmp.Pace = loopsPace[loopsPace.Count - 1];
                                    //    }
                                }
                            }
                        }
                    }

                    int newDist = (int)Math.Round(curDistance / unitDist, 0);

                    if (newDist != preDist)
                    {
                        if (preDist >= 0)
                        {
                            if (Ns > 0)
                            {
                                vS = sumS / Ns;
                            }
                            else
                            {
                                vS = 0;
                            }

                            if (Na > 0)
                            {
                                vA = sumA / Na;
                            }
                            else
                            {
                                vA = 0;
                            }

                            //if (vA < minAltitude) vA = minAltitude;
                            var currentId = current.Id;
                            double res = 1;
                            if (sessionObj.Duration.HasValue)
                            {
                                if (sessionObj.ObjTime.HasValue && sessionObj.ObjDistance.HasValue)
                                {
                                    res = 1 - (curDistance * sessionObj.ObjTime.Value) / (sessionObj.ObjDistance.Value * (new TimeSpan(currentId - firstTime).TotalSeconds));
                                }
                                else
                                {
                                    res = 1 - (curDistance * sessionObj.Duration.Value) / (sessionObj.Distance.Value * (currentId - firstTime));
                                }
                            }


                            int T = newDist - preDist;

                            PreviousMesure.Altitude = vA;
                            PreviousMesure.Distance = curDistance; // Fixme!
                            if (sessionObj.Duration.HasValue)
                            {
                                PreviousMesure.Time = currentId - firstTime;
                            }
                            else
                            {
                                PreviousMesure.Time = 0;
                            }
                            PreviousMesure.Speed = vS;
                            PreviousMesure.Rhythm = res;
                            mesuresList.Add(PreviousMesure);
                        }

                        if (current.Altitude.HasValue) // && current.altitude.Value >= ALTITUDE_LIMIT_MIN && current.altitude.Value <= ALTITUDE_LIMIT)
                        {
                            Na = 1;
                            sumA = current.Altitude.Value;
                        }
                        else
                        {
                            Na = 0;
                            sumA = 0;
                        }
                        if (current.Speed.HasValue) // && current.speed.Value > 0 && current.speed.Value <= SPEED_LIMIT)
                        {
                            Ns = 1;
                            sumS = current.Speed.Value;
                        }
                        else
                        {
                            Ns = 0;
                            sumS = 0;
                        }
                        preDist = newDist;
                        PreviousMesure = NewMesure;
                    }
                    else
                    {
                        if (current.Altitude.HasValue) // && current.altitude.Value >= ALTITUDE_LIMIT_MIN && current.altitude.Value <= ALTITUDE_LIMIT)
                        {
                            Na++;
                            sumA += current.Altitude.Value;
                        }

                        if (current.Speed.HasValue) // && current.speed.Value > 0 && current.speed.Value <= SPEED_LIMIT)
                        {
                            Ns++;
                            sumS += current.Speed.Value;
                        }
                    }

                }
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 6");
#endif

                if (Na > 0 && PreviousMesure != null)
                {
                    var currentId = PreviousMesure.Id;
                    double d = curDistance - previousLoopDistance;
                    long t = PreviousMesure.Id - previousLoopId;
                    if (d > 0 && t > 0)
                    {
                        LapDetail tmpLap = new LapDetail() { Position = PreviousMesure.Position, Distance = curDistance, Pace = Others.Utils.toPace(t, d), Time = currentId - firstTime/*, Tendency = tendency*/ };
                        lapsDetail.Add(tmpLap);
                    }

                    int newDist = (int)Math.Round(curDistance / unitDist, 0);

                    if (Ns > 0)
                    {
                        vS = sumS / Ns;
                    }
                    else
                    {
                        vS = 0;
                    }

                    if (Na > 0)
                    {
                        vA = sumA / Na;
                    }
                    else
                    {
                        vA = 0;
                    }

                    //if (vA < minAltitude) vA = minAltitude;

                    double res = 1;
                    if (sessionObj.Duration.HasValue)
                    {
                        if (sessionObj.ObjTime.HasValue && sessionObj.ObjDistance.HasValue)
                        {
                            res = 1 - (curDistance * sessionObj.ObjTime.Value) / (sessionObj.ObjDistance.Value * (new TimeSpan(currentId - firstTime).TotalSeconds));
                        }
                        else
                        {
                            res = 1 - (curDistance * sessionObj.Duration.Value) / (sessionObj.Distance.Value * (currentId - firstTime));
                        }
                    }

                    int T = newDist - preDist;

                    PreviousMesure.Altitude = vA;
                    PreviousMesure.Distance = curDistance; // Fixme!
                    if (sessionObj.Duration.HasValue)
                    {
                        PreviousMesure.Time = currentId - firstTime;
                    }
                    else
                    {
                        PreviousMesure.Time = 0;
                    }
                    PreviousMesure.Speed = vS;
                    PreviousMesure.Rhythm = res;

                    mesuresList.Add(PreviousMesure);
                }

#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 8");
#endif
                foreach (var tmp in mesuresList)
                {
                    if (!double.IsNaN(tmp.Speed))
                    {
                        MinSpeed = Math.Min(tmp.Speed, MinSpeed);
                        MaxSpeed = Math.Max(tmp.Speed, MaxSpeed);
                    }
                    if (!double.IsNaN(tmp.Altitude))
                    {
                        MinAltitude = Math.Min(tmp.Altitude, MinAltitude);
                        MaxAltitude = Math.Max(tmp.Altitude, MaxAltitude);
                    }
                }

                SumPace = 0;
                double previousPace = double.NaN;
                foreach (var tmp in lapsDetail)
                {
                    if (!double.IsNaN(tmp.Pace))
                    {
                        SumPace += tmp.Pace;
                        MinPace = Math.Min(tmp.Pace, MinPace);
                        MaxPace = Math.Max(tmp.Pace, MaxPace);

                        int tendency = -1;
                        double currentPace = Math.Round(tmp.Pace, 1);
                        if (!double.IsNaN(previousPace))
                        {
                            if (currentPace > previousPace) tendency = 1;
                            else if (currentPace < previousPace) tendency = -1;
                            else tendency = 0;

                        }
                        else
                        {
                            // first depents on planned pace
                            if (sessionObj.Duration.HasValue)
                            {
                                if (sessionObj.AvgPace.HasValue)
                                {
                                    double b = Math.Round(sessionObj.AvgPace.Value, 1);
                                    if (currentPace > b) tendency = 1;
                                    else if (currentPace < b) tendency = -1;
                                    else tendency = 0;
                                }
                            }
                        }
                        tmp.Tendency = tendency;
                        previousPace = currentPace;
                    }
                }

                if (lapsDetail.Count > 0)
                {
                    AvgPace = SumPace / (double)lapsDetail.Count;
                }

                ctlLapsVisibility = lapsDetail.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                LapsDetail = lapsDetail;
                MesuresList = mesuresList;

#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Step 9");
#endif

                summaryData = string.Format(resourceLoader.GetString("summaryData"), totalData.ToString(), ignore.ToString());


                if (totalData > 1)
                {
                    var mapElements = new List<object>();
                    mapElements.Add(new TextMap
                    {
                        Location = new Geopoint(mesuresList[0].Position.NoAltitude()),
                        Title = sessionObj.Duration.HasValue ? Others.Utils.toDistanceString(0, unit, CultureInfo.CurrentCulture) + "\n" + Others.Utils.toTimeString(mesuresList[0].Time) : Others.Utils.toDistanceString(0, unit, CultureInfo.CurrentCulture),
                        Tag = 0,
                        StyleText = Application.Current.Resources["PUSHPIN_DISTANCE_STYLE_START"] as Style,
                        PanelStyleText = Application.Current.Resources["PUSHPIN_PANEL_DISTANCE_STYLE_START"] as Style
                    });
                    var last = mesuresList.Count - 1;
                    if (last > 0)
                    {
                        mapElements.Add(new TextMap
                        {
                            Location = new Geopoint(mesuresList[last].Position.NoAltitude()),
                            Title = sessionObj.Duration.HasValue ? Others.Utils.toDistanceString(mesuresList[last].Distance, unit, CultureInfo.CurrentCulture) + "\n" + Others.Utils.toTimeString(sessionObj.Duration.Value) : Others.Utils.toDistanceString(mesuresList[last].Distance, unit, CultureInfo.CurrentCulture),
                            Tag = 0,
                            StyleText = Application.Current.Resources["PUSHPIN_DISTANCE_STYLE_END"] as Style,
                            PanelStyleText = Application.Current.Resources["PUSHPIN_PANEL_DISTANCE_STYLE_END"] as Style
                        });
                    }
#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("Step 10");
#endif
                    if (lapsDetail.Count() > 1)
                    {
                        ctlLapsVisibility = Visibility.Visible;

                        int lapI = 0;
                        foreach (var tmp in lapsDetail)
                        {
                            int newDist = (int)Math.Round(tmp.Distance / unitDist, 0);

                            mapElements.Add(new LoopTextMap()
                            {
                                Location = new Geopoint(tmp.Position.NoAltitude()),
                                Title = Others.Utils.toDistanceStringShort(tmp.Distance, unit, CultureInfo.CurrentCulture),
                                Tag = lapI,
                                StyleText = Application.Current.Resources["PUSHPIN_LOOP_STYLE"] as Style,
                                PanelStyleText = Application.Current.Resources[tmp.Tendency == 0 ? "LPACE_TXT_STYLE" : tmp.Tendency > 0 ? "LPACE_TXT_STYLE_N" : "LPACE_TXT_STYLE_P"] as Style,
                                Tapped = Loop_Tapped
                            });
                            lapI++;
                            // TODO

                            //    MapOverlay tmp = new MapOverlay();
                            //    tmp.PositionOrigin = new Point(0.5, 0.5);
                            //    tmp.Content = panel; //txt;
                            //    panel.Tag = i;
                            //    panel.Tapped += img_Tap;
                            //    tmpLap.pos = pos;
                            //    tmp.GeoCoordinate = pos;
                        }
                    }

#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("Step 13");
#endif
                    var dynamicPolyline = new List<BasicGeoposition>();

                    double previousDistanceL = double.NaN;

                    Color prevcol = Colors.Green;
                    int i = 0;
                    foreach (var tmp in mesuresList)
                    {
                        Color col = "I".Equals(mode) ? Colors.Green : Colors.Green;
                        double distanceL = Math.Round(tmp.Distance / 1000, 0); // (long)Math.Round(curDistance) / IntervalDistance;
                        if (sessionObj.Duration.HasValue)
                        {
                            col = Others.Utils.getColor(tmp.Rhythm, margin);
                        }

                        if (double.IsNaN(previousDistanceL))
                        {
                            dynamicPolyline.Add(tmp.Position.NoAltitude());
                        }
                        else
                        {
                            if (lapsDetail.Count() <= 1)
                            {
                                if (distanceL != previousDistanceL)
                                {
                                    mapElements.Add(new TextMap
                                    {
                                        Location = new Geopoint(tmp.Position.NoAltitude()),
                                        Title = sessionObj.Duration.HasValue ? Others.Utils.toDistanceString(tmp.Distance, unit, CultureInfo.CurrentCulture) + "\n" + Others.Utils.toTimeString(tmp.Time) : Others.Utils.toDistanceString(tmp.Distance, unit, CultureInfo.CurrentCulture),
                                        Tag = i,
                                        StyleText = Application.Current.Resources["PUSHPIN_DISTANCE_STYLE"] as Style,
                                        PanelStyleText = Application.Current.Resources["PUSHPIN_PANEL_DISTANCE_STYLE"] as Style
                                    });

                                }
                            }

                            if (prevcol != col)
                            {
                                mapElements.Add(new PolylineMap() { MapRoute = new Geopath(dynamicPolyline), StrokeThickness = "I".Equals(mode) ? 6 : 4, StrokeColor = prevcol });
                                var before = dynamicPolyline.Last();
                                dynamicPolyline = new List<BasicGeoposition>();
                                dynamicPolyline.Add(before.NoAltitude());
                                dynamicPolyline.Add(tmp.Position.NoAltitude());
                            }
                            else
                            {
                                dynamicPolyline.Add(tmp.Position.NoAltitude());
                            }
                        }

                        prevcol = col;
                        previousDistanceL = distanceL;
                        i++;
                    }

                    if (dynamicPolyline.Count > 0)
                        mapElements.Add(new PolylineMap() { MapRoute = new Geopath(dynamicPolyline), StrokeThickness = "I".Equals(mode) ? 6 : 4, StrokeColor = prevcol });

                    ShowInfo.Location = new Geopoint(MesuresList[0].Position.NoAltitude());

                    mapElements.Add(ShowInfo);

#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("Step 14");
#endif
                    LandmarkLayer = mapElements;

                    // TODO add current point
                    //var posS = new BasicGeoposition() { Latitude = listPoint[0].Latitude, Longitude = listPoint[0].Longitude };
                    //var pikePlaceIconS = new MapIcon
                    //{
                    //    Location = new Geopoint(posS),
                    //    Title = sessionObj.Duration.HasValue ? Others.Utils.toDistanceString(0, unit, CultureInfo.CurrentCulture) + "\n" + Others.Utils.toTimeString(listTime[0]) : Others.Utils.toDistanceString(0, unit, CultureInfo.CurrentCulture)
                    //};
                    //mapElements.Add(pikePlaceIconS);

                    // TODO https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/MapControl/shared/Scenario3.xaml
                    //{
                    //    oneMarkerPoint = new Pushpin();
                    //    oneMarkerPoint.Tag = 0;
                    //    StackPanel panel = new StackPanel();
                    //    ttext = new TextBlock();
                    //    panel.Children.Add(new Ellipse { Fill = new SolidColorBrush(Colors.Red), Width = 40, Height = 40 });
                    //    panel.Children.Add(ttext);

                    //    oneMarkerPoint.Content = panel;  // new BitmapImage(new Uri("/Images/map.checkin.png", UriKind.Relative));

                    //    //oneMarkerPoint.
                    //    oneMarkerPoint.Opacity = 0.5;

                    //    tmpPoint = new MapOverlay();
                    //    tmpPoint.PositionOrigin = new Point(0, 1);
                    //    tmpPoint.Content = oneMarkerPoint;
                    //    tmpPoint.GeoCoordinate = listPoint[0];


                    //    markerLayer.Add(tmpPoint);
                    //}

                    // https://github.com/Microsoft/Windows-universal-samples/blob/master/Samples/MapControl/shared/Scenario2.xaml


#if (DEBUG)
                    System.Diagnostics.Debug.WriteLine("Step 15");
#endif
                    fixView();
                }


                visibleMode = 0;
                if (totalData > 1)
                {
                    ctlPerfileVisibility = Visibility.Visible;
                    ctlMapVisibility = Visibility.Visible;
                }

                progressBarInitialVisibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                nowhereman.LittleWatson.instance.Error("loadData", ex);

            }
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("END LOAD DATA");
#endif

        }

        private void fixView()
        {
            MapCenter = new Geopoint(new BasicGeoposition() { Latitude = sessionObj.CenterLat.Value, Longitude = sessionObj.CenterLon.Value, Altitude = AltitudeMax }.NoAltitude());
            Scene = MapScene.CreateFromBoundingBox(new GeoboundingBox(new BasicGeoposition() { Latitude = sessionObj.MaxLat.Value, Longitude = sessionObj.MaxLon.Value, Altitude = AltitudeMax }.NoAltitude()
                                                , new BasicGeoposition() { Latitude = sessionObj.MinLat.Value, Longitude = sessionObj.MinLon.Value, Altitude = AltitudeMax }.NoAltitude()
                                                ));
        }

        private void SeeLoop(int pos)
        {
            LapDetailSelected = LapsDetail[pos];
        }

        private void showAt(double newLeft)
        {
            int pos = -1;
            try
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("SHOW AT " + newLeft);
#endif
                if (newLeft < 0) return;

                double dist = newLeft * horizontalUnit;
                int i = 0, N = MesuresList.Count;
                for (; i < N; i++)
                {
                    if (dist <= MesuresList[i].Distance)
                    {
                        pos = i;
                        break;
                    }
                }

                if (pos >= N) pos = N - 1;

                ShowInfo.altitudePoint = MesuresList[pos].Altitude;
                if (sessionObj.Duration.HasValue)
                {
                    ShowInfo.speedPoint = MesuresList[pos].Speed;
                    ShowInfo.time = new TimeSpan(MesuresList[pos].Time);
                }

                ShowInfo.point = MesuresList[pos].Distance;

                ShowInfo.Top = spaceSpeed;
                ShowInfo.Left = newLeft + newLeft < altitudeCanvasWidth / 2 ? 15 : -130;

                PointToAltitude.Top = graphPointsCollectionAltitude[pos].Y - POINT_WIDTH;
                PointToAltitude.Left = newLeft - POINT_WIDTH;

                if (sessionObj.Duration.HasValue && pos < graphPointsCollectionSpeed.Count)
                {
                    PointToSpeed.Top = graphPointsCollectionSpeed[pos].Y - POINT_WIDTH;
                    PointToSpeed.Left = newLeft - POINT_WIDTH;
                }

                ShowInfo.Location = new Geopoint(MesuresList[pos].Position.NoAltitude());
            }
            catch (Exception e)
            {
                nowhereman.LittleWatson.instance.Error("showAt " + pos + " " + newLeft, e);
            }
        }
        private void MapTypeCommand()
        {
            CurrentMapStyle = (MapStyle)(((int)CurrentMapStyle + 1) % 7);
        }

        private bool CompressAction()
        {
            DataBaseManager.instance.compressData(sessionObj);


            return false;
        }

        private bool FixAction()
        {
            try
            {
                string mode = pathObj.Type;
                if (mode == "I")
                {
                    return false;
                }

                var sessions2 = DataBaseManager.instance.GetMesures(sessionObj.Id);

                bool fixDataWithPitagoras = nowhereman.Properties.getBoolProperty("fixDataWithPitagoras" + mode, false);

                double N = 0;
                Mesures firstMesure = null;
                BasicGeoposition prev = new BasicGeoposition();

                double currentDistance = 0;
                double Na = 0, Ns = 0;
                double centerLat = 0;
                double centerLon = 0;

                double ascendent = 0;
                double descendent = 0;

                double avgSpeed = 0;
                double avgAltitude = 0;

                double _previousAltitude = 0;

                double maxAltitude = Double.MinValue;
                double minAltitude = Double.MaxValue;

                double maxLat = Double.MinValue;
                double maxLon = Double.MinValue;
                double minLat = Double.MaxValue;
                double minLon = Double.MaxValue;

                double minSpeed = Double.MaxValue;
                double maxSpeed = Double.MinValue;

                Mesures previousMesure = null;
                long timeToIgnore = 0;
                double? previousAltitude = 0;

                foreach (Mesures current in sessions2)
                {
                    N++;

                    if (current.Action != null && "RESUME".Equals(current.Action) && previousMesure != null)
                    {
                        timeToIgnore += previousMesure.Id - current.Id;
                    }

                    if (firstMesure == null)
                    {
                        firstMesure = current;
                    }

                    double altitude = (current.Altitude.HasValue /*&& current.altitude >= ALTITUDE_LIMIT_MIN && current.altitude <= ALTITUDE_LIMIT*/) ? current.Altitude.Value : 0;

                    BasicGeoposition _POS = new BasicGeoposition() { Latitude = current.Latitude, Longitude = current.Longitude, Altitude = altitude };
                    if (previousMesure != null)
                    {
                        double dist = prev.GetDistanceTo(_POS);
                        if (fixDataWithPitagoras)
                        {
                            if (prev.Altitude > 0 && _POS.Altitude > 0 && !Double.IsNaN(prev.Altitude) && !Double.IsNaN(_POS.Altitude))
                            {
                                double tmp = prev.Altitude - _POS.Altitude;
                                double delta = Math.Sqrt(tmp * tmp + dist * dist);

                                dist = delta;
                            }
                        }

                        currentDistance += dist;
                    }
                    else
                    {
                        sessionObj.StartLat = current.Latitude;
                        sessionObj.StartLon = current.Longitude;
                    }
                    _previousAltitude = altitude;
                    prev = _POS;

                    if (current.Speed.HasValue)
                    {
                        double speed = current.Speed.Value;

                        minSpeed = Math.Min(minSpeed, speed);
                        maxSpeed = Math.Max(maxSpeed, speed);
                        if (speed != 0) avgSpeed += 1 / speed;
                        Ns++;
                    }

                    if (current.Altitude.HasValue)
                    {
                        minAltitude = Math.Min(minAltitude, altitude);
                        maxAltitude = Math.Max(maxAltitude, altitude);
                        if (altitude != 0) avgAltitude += 1 / altitude;
                        Na++;
                    }

                    centerLat += current.Latitude;
                    centerLon += current.Longitude;

                    maxLat = Math.Max(maxLat, current.Latitude);
                    minLat = Math.Min(minLat, current.Latitude);

                    maxLon = Math.Max(maxLon, current.Longitude);
                    minLon = Math.Min(minLon, current.Longitude);

                    if (current.Altitude.HasValue)
                    {
                        if (previousAltitude.HasValue)
                        {
                            if (Math.Abs(previousAltitude.Value - altitude) >= MIN_DELTA_ALTITUDE)
                            {
                                if (previousAltitude.Value < altitude)
                                {
                                    ascendent += altitude - previousAltitude.Value;
                                }
                                else
                                {
                                    descendent += previousAltitude.Value - altitude;
                                }
                                previousAltitude = altitude;
                            }
                        }
                        else
                        {
                            previousAltitude = altitude;
                        }
                    }

                    previousMesure = current;
                }

                if (firstMesure != null)
                {
                    long currentTime = previousMesure.Id - firstMesure.Id - timeToIgnore;

                    sessionObj.EndLat = previousMesure.Latitude;
                    sessionObj.EndLon = previousMesure.Longitude;

                    sessionObj.MinSpeed = minSpeed;
                    sessionObj.MaxSpeed = maxSpeed;
                    sessionObj.MinAltitude = minAltitude;
                    sessionObj.MaxAltitude = maxAltitude;
                    sessionObj.AvgAltitude = avgAltitude != 0.0 ? Na / avgAltitude : 0.0;
                    sessionObj.AvgSpeed = avgSpeed != 0.0 ? Ns / avgSpeed : 0.0;

                    sessionObj.CenterLat = centerLat / N;
                    sessionObj.CenterLon = centerLon / N;
                    sessionObj.MaxLat = maxLat;
                    sessionObj.MaxLon = maxLon;
                    sessionObj.MinLat = minLat;
                    sessionObj.MinLon = minLon;

                    sessionObj.Ascendent = ascendent;
                    sessionObj.Descendent = descendent;
                    sessionObj.Distance = currentDistance;
                    sessionObj.Duration = currentTime;
                    sessionObj.AvgPace = Utils.toPace(currentTime, currentDistance);

                    DataBaseManager.instance.UpdateSession(sessionObj);

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("fixing data", ex);
                return false;
            }
        }

        private void DeleteAction()
        {
            DataBaseManager.instance.DeleteSession(sessionObj, true);
            NavigationService.GoBack();
        }
    }
}

