using SoftConsept.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Others;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;

namespace UniversalKeepTheRhythm.Services
{
#if(DEBUGx)

    public class Geolocator
    {
        public PositionAccuracy DesiredAccuracy {get; set;}
        public uint ReportInterval {get; set;}
        public double MovementThreshold { get; set; }

        public delegate void StatusChangedDelegate(PositionStatus args);
        public delegate void PositionChangedDelegate(long utcTicks, Geocoordinate eloc);

        public event StatusChangedDelegate StatusChanged;
        public event PositionChangedDelegate PositionChanged;

        private Task task;

        public Geolocator()
        {
            task = Task.Factory.StartNew(() =>
            {
                int N = 10;
                while (StatusChanged == null || PositionChanged == null)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                StatusChanged(PositionStatus.Ready);

                try
                {
                    char [] sep = {';'};
                    long _UtcTicks = -1;

                    //string name = "debug_20140428_1925.txt";
                    //string name = "debug_20140426_2343.txt";
                    //string name = "debug_20140504_1718.txt";
                    string name = "debug_20140504_1847.txt";

                    using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isoFileStream = new StreamReader(new IsolatedStorageFileStream(name, FileMode.Open, store)))
                        {
                            //int C = 2100;
                            while (true)
                            {
                                String line = isoFileStream.ReadLine();

                                if (line == null || line == "") break;

                                //if (--C >= 0) continue;

                                String [] values = line.Split(sep);

                                long UtcTicks = long.Parse(values[0],CultureInfo.InvariantCulture.NumberFormat);
                                double _Latitude = double.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat);
                                double _Longitude = double.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
                                double _Accuracy = double.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat);
                                double _VerticalAccuracy = double.Parse(values[4], CultureInfo.InvariantCulture.NumberFormat);
                                double _Altitude = double.Parse(values[5], CultureInfo.InvariantCulture.NumberFormat);
                                double _Course = double.Parse(values[6], CultureInfo.InvariantCulture.NumberFormat);
                                double _Speed = double.Parse(values[7], CultureInfo.InvariantCulture.NumberFormat);

                                Geocoordinate args = new Geocoordinate()
                                {
                                    Altitude = _Altitude, Speed = _Speed, Course = _Course, Accuracy = _Accuracy, VerticalAccuracy = _VerticalAccuracy, Latitude = _Latitude, Longitude=_Longitude
                                };

                                if (N > 0)
                                {
                                    long n = UtcTicks - (N+2)*1000;
                                    while (--N > 0)
                                    {
                                        PositionChanged(n, args);
                                        Thread.Sleep(TimeSpan.FromSeconds(1));
                                        n += 1000;
                                    }
                                    N = 0;
                                }

                                PositionChanged(UtcTicks, args);

                                if (_UtcTicks != -1)
                                {
                                    if (UtcTicks - _UtcTicks > 0)
                                        Thread.Sleep(TimeSpan.FromTicks(UtcTicks - _UtcTicks));
                                }
                                _UtcTicks = UtcTicks;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    nowhereman.LittleWatson.instance.Error("reader", ex);
                }
            });

        }
    }
#endif

    public class Engine : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled) return;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        //public Settings sets
        //{
        //    get
        //    {
        //        return Settings.instance();
        //    }
        //}

#if(DEBUG)
        bool storeAllData = true;
#else
        bool storeAllData = false;
#endif

        Geolocator watcher;

        private bool fixDataWithPitagoras = true;

        public bool started { get; set; }
        public bool running { get; set; }
        public bool paused { get; set; }
        public bool wasStarted { get; set; }
        public bool wasObscured { get; set; }

        static private int BUFFERS_LEN = 5;

        // delegate declaration 
        public delegate void PositionChanged(Engine eng, TimeSpan time, double speed, double altitude, double distance, double ascendent, double descendent, double distanceFromStart, double distanceToEnd, Geocoordinate e);
        public delegate void StatusChanged(Engine eng, string status, double? hacc, double? vacc);

        public delegate void ExecutePopupAction(Engine eng, _TriggerAction action, bool show);
        public delegate void ExecuteAction(Engine eng, _TriggerAction action);
        public delegate void ExecuteActionA(Engine eng, string action);

        public delegate void NotifyMovement(Engine eng, int S, int U, int R);

        // event declaration 
        public event PositionChanged handlerP;
        public event StatusChanged handlerS;

        public event ExecutePopupAction popup;
        public event ExecuteAction handler;
        public event ExecuteActionA handlerA;

        public event NotifyMovement notifyMovement;

        private StreamWriter isoFileStream = null;

        private string mode;
        private double expectedDistance = 5;

        string actionMsg = null;

        Sessions sessionData;
        Paths path;

        ListOfMovements movements;

        int loopCount = 0;

        bool isNew;
        bool restart = false;
        Timer timer = null, timerIntelligence = null;
        long startTime;
        long PACEstartTime;
        List<long> LOOPstartTime = new List<long>();

        bool isLoop = false;
        double loopDistance = Double.NaN;

        double ObjPace;

        bool hasGPSEnabled;
        bool receivedData;
        long lastPositionTime = -1;
        double previousAcuraccy = Double.MaxValue;
        double previousVAcuraccy = Double.MaxValue;
        double? previousSpeed = null;
        double? previousAltitude = null;
        Geocoordinate lastAltitude = null;

        long currentTime;

        double pauseDistance = 0;
        Geocoordinate pausedPosition = null;

        long pauseTime = 0;
        long despTime = 0;
        bool waitToStart;

        bool objectiveSet = false;
        long lastAdvice = -1;

        bool intelligence = true;
        bool intelligencePace = true;

        double currentDistance = 0;
        double despDistance = 0;
        double PACEcurrentDistance = 0;
        List<double> LOOPcurrentDistance = new List<double>();

        double LIMIT = 0.05;
        double MIN_MINUTE = 1;
        double MAX_ACCURACY = 50;

        int adviceNumber = 0;
        int limitAdviceNumber = 3;

        int previousRes = 0;

        bool dataLost = false;
        bool dataCollection = false;

        Geocoordinate previousPoint = null;

        Geocoordinate initialPoint = null;
        Geocoordinate higherPoint = null;
        Geocoordinate lowerPoint = null;
        Geocoordinate lastReceived = null;
        //Geocoordinate reallyLastReceived = null;

        double higherPointDistance, lowerPointDistance;
        long higherPointTime, lowerPointTime;

        //nowhereman.CircularBuffer<Info> buffer;
        //long timeToEnd;

        bool starting = true;

        bool requiredAccuracyTime = false;
        double requiredAccuracy;
        double HorAccuracy = Double.MaxValue;
        double? VerAccuracy = Double.MaxValue;

        CircularBuffer<Geocoordinate> lastPositions;

        ListOfValues distanceFromStart;
        ListOfValues distanceToEnd;
        ListOfInmediatePoints pints;

        public string unit { get; private set; }

        double centerLat = 0;
        double centerLon = 0;

        double ascendent = 0;
        double descendent = 0;

        double avgSpeed = 0;
        double avgAltitude = 0;

        double maxAltitude = Double.MinValue;
        double minAltitude = Double.MaxValue;

        double maxLat = Double.MinValue;
        double maxLon = Double.MinValue;
        double minLat = Double.MaxValue;
        double minLon = Double.MaxValue;

        double minSpeed = Double.MaxValue;
        double maxSpeed = Double.MinValue;

        long N = 0, Na = 0, Ns = 0;

        public SortedCollection<TriggerActionTime> timeActions = new SortedCollection<TriggerActionTime>(); // Sorted list
        int timeActionPoint = -1;

        public SortedCollection<TriggerActionDistance> distanceActions = new SortedCollection<TriggerActionDistance>(); // Sorted list
        int distanceActionPoint = -1;

        TimeSpan objectiveTime = TimeSpan.MinValue;
        double objectiveDistance = Double.NaN;

        //TimeSpan PACEobjectiveTime = TimeSpan.MinValue;
        //double PACEobjectiveDistance = Double.NaN;

        double startLatitude = Double.NaN, startLongitude = Double.NaN;
        double endLatitude = Double.NaN, endLongitude = Double.NaN;

        long previousP = -1;
        double objectiveSpeed = Double.NaN;
        int SECONDS_TO_DISTANCE = 5;
        int intervalDistance = 0;
        int intervalTime = 0;
        long counter = 0;

        public bool obscured = false;
        bool wasPaused = false;

        double ALTITUDE_LIMIT;
        double ALTITUDE_LIMIT_MIN;
        //double SPEED_LIMIT;

        bool storeAllPoints = false;

        //BackgroundWorker bw = null;

        CancellationTokenSource tokenSource = null;
        Task monitor;

        readonly object _locker = new object();
        Queue<object> command = new Queue<object>();

        private static int MOVEMENT_REFRESH = 10;
        private static int DELAY_REFRESH = 5;
        private static int REFRESH_EVERY = 200;

        private int TIME_TO_PAUSE = 5;
        private int TIME_TO_RUN = 3;

        private bool hasObjective = false;


        private static readonly object SyncRoot = new object();

        private static Engine _instance = null;
        public static Engine instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new Engine();
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        public void setStartPoint(double lat, double lon)
        {
            startLatitude = lat;
            startLongitude = lon;
        }

        public void setEndPoint(double lat, double lon)
        {
            endLatitude = lat;
            endLongitude = lon;
        }

        public void setIsLoop(bool _isLoop)
        {
            isLoop = _isLoop;
            loopDistance = Double.NaN;
        }

        public void setIsLoop(bool _isLoop, double _distance)
        {
            isLoop = _isLoop;
            loopDistance = _distance;
        }

        public void setObjective(double distance, TimeSpan t)
        {
            objectiveDistance = distance;
            objectiveTime = t;

            if (!Double.IsNaN(objectiveDistance) && objectiveTime != TimeSpan.MinValue && objectiveTime.TotalSeconds > 0 && objectiveDistance > 0)
            {
                objectiveSpeed = objectiveDistance / objectiveTime.TotalSeconds;

                double totalSeconds = t.TotalSeconds;
                ObjPace = totalSeconds / distance;

                hasObjective = true;
            }

        }

        public double? getObjectiveSpeed()
        {
            if (!Double.IsNaN(objectiveSpeed))
            {
                return objectiveSpeed;
            }
            else
            {
                return null;
            }
        }

        public double getObjectiveDistanceRemaining()
        {
            if (!Double.IsNaN(objectiveDistance) && objectiveDistance > 0)
            {
                return objectiveDistance - currentDistance;
            }
            else
            {
                return Double.NaN;
            }
        }

        public TimeSpan getObjectiveDurationRemaining()
        {
            if (objectiveTime != TimeSpan.MinValue && objectiveTime.TotalSeconds > 0)
            {
                if (objectiveTime.Ticks > currentTime)
                    return new TimeSpan(objectiveTime.Ticks - currentTime);
                else
                    return new TimeSpan(0);
            }
            else
            {
                return TimeSpan.MinValue;
            }
        }

        public Sessions getSession()
        {
            return sessionData;
        }


        public void background()
        {
            if (nowhereman.Properties.getBoolProperty("PauseOnObscured" + mode, false))
            {
                if (!obscured)
                {
                    obscured = true;
                    wasPaused = paused;
                    if (!wasPaused)
                    {
                        pause(false);
                    }
                }
            }
            else
            {
                if (wasStarted && running)
                {
                    //Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
                    ///*if (hasGPS)
                    //{
                    //    toast.Content = toDistanceString(_distance) + "\n" + toTimeString(_time);
                    //}
                    //else
                    //{
                    //    toast.Content = toTimeString(_time);
                    //}*/
                    //toast.Content = AppResources.BackgroundMessage;
                    //toast.Title = AppResources.app_name;
                    ////toast.NavigationUri = new Uri("/MainPage.xaml?param=return", UriKind.Relative);
                    ////toast.NavigationUri = new Uri("/Constants.xaml?param=return", UriKind.Relative);
                    //toast.Show();
                }
                else
                {
                    if (!obscured)
                    {
                        obscured = true;
                        wasPaused = paused;
                        if (!wasPaused)
                        {
                            pause(false);
                        }
                    }
                }
            }
        }

        public void activate()
        {
            if (obscured)
            {
                obscured = false;
                if (!wasPaused)
                {
                    resume(false);
                }
            }
        }

        public void deactivate()
        {
            if (nowhereman.Properties.getBoolProperty("PauseOnObscured" + mode, false))
            {
                if (!obscured)
                {
                    obscured = true;
                    wasPaused = paused;
                    if (!wasPaused)
                    {
                        pause(false);
                    }
                }
            }
        }

        // https://msdn.microsoft.com/en-us/library/windows/apps/hh465148.aspx?f=255&MSPPError=-2147217396
        private ExtendedExecutionSession session;

        private async void StartLocationExtensionSession()
        {
            session = new ExtendedExecutionSession();
            session.Description = "Location Tracker";
            session.Reason = ExtendedExecutionReason.LocationTracking;
            session.Revoked += ExtendedExecutionSession_Revoked;
            var result = await session.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Denied)
            {
                //TODO: handle denied
            }
        }

        private void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        public async Task<int> start(Paths pathData, bool hasGPS, double _avgSpeed, bool timeAcc, string desc, int idSession)
        {
            movements = new ListOfMovements(Math.Max(TIME_TO_PAUSE, TIME_TO_RUN)); // * 1000 / REFRESH_EVERY);
            wasStarted = false;
            path = pathData;
            mode = pathData.Type;

            unit = nowhereman.Properties.getProperty("units", "m");

            fixDataWithPitagoras = nowhereman.Properties.getBoolProperty("fixDataWithPitagoras" + mode, false);

            TIME_TO_PAUSE = nowhereman.Properties.getIntProperty("timeToPause" + mode, 5);
            TIME_TO_RUN = nowhereman.Properties.getIntProperty("timeToRun" + mode, 3);

            intelligence = nowhereman.Properties.getBoolProperty("intelligence" + mode, true);
            intelligencePace = nowhereman.Properties.getBoolProperty("intelligencePace" + mode, true);

            storeAllPoints = nowhereman.Properties.getBoolProperty("storeAllPoints", false);

            MAX_ACCURACY = nowhereman.Properties.getDoubleProperty("maxAccuracy" + mode, MAX_ACCURACY);

            ALTITUDE_LIMIT = nowhereman.Properties.getDoubleProperty("maxAltitude" + mode, 8848);
            ALTITUDE_LIMIT_MIN = nowhereman.Properties.getDoubleProperty("minAltitude" + mode, -10);
            //SPEED_LIMIT = nowhereman.Properties.getDoubleProperty("maxSpeed" + mode, MaxSpeed(mode));

            SECONDS_TO_DISTANCE = nowhereman.Properties.getIntProperty("secondsToDistance" + mode, 5);
            LIMIT = nowhereman.Properties.getDoubleProperty("IntervalForLimit" + mode, 0.05);
            MIN_MINUTE = nowhereman.Properties.getDoubleProperty("MinutesEveryAdvice" + mode, 1);
            limitAdviceNumber = nowhereman.Properties.getIntProperty("limitAdviceNumber" + mode, 3);

            timeActions.Add(new TriggerActionTime(this) { action = Constants.STARTING, time = new TimeSpan(0, 0, 0) });

            intervalTime = nowhereman.Properties.getIntProperty("timeAdvicesEveryMinute" + mode, 15);
            intervalDistance = nowhereman.Properties.getIntProperty("distanceAdvicesEveryMeter" + mode, 1000);

            if (intervalTime > 0)
            {
                int h = (intervalTime / 60);
                int m = (intervalTime % 60);
                timeActions.Add(new TriggerActionTime(this) { action = Constants.MINUTES_FROM, time = new TimeSpan(h, m, 0) });

                //                                        for (int i = interval; i <= (int)Math.Floor(objectiveTime.TotalMinutes / 2); i += interval)
            }

            if (objectiveTime != TimeSpan.MinValue)
            {
                timeActions.Add(new TriggerActionTime(this) { action = Constants.HALF_TIME_FROM, time = new TimeSpan(objectiveTime.Ticks / 2) });
                timeActions.Add(new TriggerActionTime(this) { action = Constants.FINISH_TIME, time = objectiveTime });

                /*if (intervalTime > 0)
                {
                    int t = (int)Math.Floor(objectiveTime.TotalMinutes / 2) + intervalTime;
                    int h = (t / 60);
                    int m = (t % 60);
                    TimeSpan tmp = new TimeSpan(h, m, 0);
                    timeActions.Add(new TriggerActionTime() { action = Constants.MINUTES_TO, time = tmp, param = objectiveTime.Subtract(tmp) });
                }*/
            }


            timeActions.Add(new TriggerActionTime(this) { action = Constants.END, time = TimeSpan.MaxValue });


            if (!Double.IsNaN(loopDistance))
            {
                distanceActions.Add(new TriggerActionDistance(this) { action = Constants.LOOP_DETECTED, distance = (double)loopDistance, param = 0L });
            }
            if (intervalDistance > 0)
            {
                distanceActions.Add(new TriggerActionDistance(this) { action = Constants.DISTANCE_FROM, distance = (double)intervalDistance });
            }

            if (!Double.IsNaN(objectiveDistance))
            {
                distanceActions.Add(new TriggerActionDistance(this) { action = Constants.HALF_DISTANCE_FROM, distance = objectiveDistance / 2 });
                /*if (intervalDistance > 0)
                {
                    double tmp = Math.Floor(objectiveDistance / 2) + intervalDistance;
                    distanceActions.Add(new TriggerActionDistance(this) { action = Constants.DISTANCE_TO, distance = tmp, param = objectiveDistance - tmp });
                }*/
                distanceActions.Add(new TriggerActionDistance(this) { action = Constants.FINISH_DISTANCE, distance = objectiveDistance });
            }

            // we add at least one never that it will happen
            distanceActions.Add(new TriggerActionDistance(this) { action = Constants.END, distance = Double.MaxValue });

            paused = false;
            starting = true;
            waitToStart = true;
            distanceFromStart = new ListOfValues(BUFFERS_LEN);
            distanceToEnd = new ListOfValues(BUFFERS_LEN);
            pints = new ListOfInmediatePoints(BUFFERS_LEN);
            //buffer = new nowhereman.CircularBuffer<Info>(BUFFERS_LEN);
            //lastReceived = null;

            lastPositions = new CircularBuffer<Geocoordinate>(50);
            lastPositionMovement = null;
            lastUnknown = 0;

            dataLost = false;
            dataCollection = true;


            if (idSession < 0)
            {
                sessionData = new Sessions();
                sessionData.IdPath = pathData.Id;
                sessionData.Comment = desc;
                sessionData.DayOfSession = Utils.NOW();

                if (!Double.IsNaN(objectiveSpeed))
                {
                    sessionData.Objective = objectiveSpeed;
                }
                else
                {
                    sessionData.Objective = null;
                }

                if (!Double.IsNaN(objectiveDistance) && objectiveDistance > 0)
                {
                    sessionData.ObjDistance = objectiveDistance;
                }
                else
                {
                    sessionData.ObjDistance = null;
                }

                if (objectiveTime != TimeSpan.MinValue && objectiveTime.TotalSeconds > 0)
                {
                    sessionData.ObjTime = (long)objectiveTime.TotalSeconds;
                }
                else
                {
                    sessionData.ObjTime = null;
                }


                //sessionData.MinLat = 0.0;
                //sessionData.MinLon = 0.0;
                //sessionData.MaxLat = 0.0;
                //sessionData.MaxLon = 0.0;
                //sessionData.Distance = 0.0;
                //sessionData.Duration = -1;
                //sessionData.MinSpeed = 0.0;
                //sessionData.MaxSpeed = 0.0;
                //sessionData.AvgSpeed = 0.0;
                //sessionData.MinAltitude = 0.0;
                //sessionData.MaxAltitude = 0.0;
                //sessionData.AvgAltitude = 0.0;
                //sessionData.CenterLat = 0.0;
                //sessionData.CenterLon = 0.0;
                //sessionData.Ascendent = 0.0;
                //sessionData.Descendent = 0.0;
                //sessionData.AvgPace = 0.0;
                //sessionData.StartLon = 0.0;
                //sessionData.StartLat = 0.0;
                //sessionData.EndLon = 0.0;
                //sessionData.EndLat = 0.0;
                //sessionData.Objective = 0.0;
                //sessionData.ObjDistance = 0.0;
                //sessionData.ObjTime = -1;


                DataBaseManager.instance.InsertSession(sessionData);

                idSession = sessionData.Id;
                isNew = true;

                despTime = 0;
                despDistance = 0.0;
            }
            else
            {
                isNew = false;
                sessionData = DataBaseManager.instance.GetSession(idSession);
                actionMsg = "C";

                despDistance = sessionData.Distance.Value;
                despTime = sessionData.Duration.Value;

                // TODO update objective???
                /*
                                 if (objectiveSpeed != Double.NaN)
                {
                    sessionData.objective = objectiveSpeed;
                }
                else
                {
                    sessionData.objective = null;
                }

                if (objectiveDistance != Double.NaN && objectiveDistance > 0)
                {
                    sessionData.objDistance = objectiveDistance;
                }
                else
                {
                    sessionData.objDistance = null;
                }

                if (objectiveTime != TimeSpan.MinValue && objectiveTime.TotalSeconds > 0)
                {
                    sessionData.objTime = (long)objectiveTime.TotalSeconds;
                }
                else
                {
                    sessionData.objTime = null;
                }
                 */
            }

            started = true;
            waitToStart = true;
            running = false;
            receivedData = false;

            hasGPSEnabled = hasGPS;
            if (hasGPS)
            {
                try
                {
                    //MOVEMENT_REFRESH = _avgSpeed / currentSpeed -> round to max

                    requiredAccuracy = _avgSpeed;
                    requiredAccuracyTime = timeAcc;

                    //startingDistance = Double.MaxValue;
                    HorAccuracy = Double.MaxValue;
                    VerAccuracy = Double.MaxValue;

                    if (mode == "C")
                    {
                        // 15km/h = >  10000/3600 = 4.2 m/s
                        expectedDistance = 4.2;
                    }
                    else if (mode == "R")
                    {
                        // 10km/h = >  10000/3600 = 2.8 m/s
                        expectedDistance = 2.8;
                    }
                    else
                    {
                        // 5km/h = > 5000/3600 = 1.4 m/s
                        expectedDistance = 1.4;
                    }

                    var accessStatus = await Geolocator.RequestAccessAsync();

                    switch (accessStatus)
                    {
                        case GeolocationAccessStatus.Allowed:
                            break;

                        case GeolocationAccessStatus.Denied:
                            return -1;

                        case GeolocationAccessStatus.Unspecified:
                            return -1;
                    }

                    if (timeAcc)
                    {
                        watcher = new Geolocator()
                        {
                            DesiredAccuracy = PositionAccuracy.High,
                            ReportInterval = (uint)TimeSpan.FromSeconds(_avgSpeed).TotalMilliseconds
                        };
                    }
                    else
                    {
                        if (nowhereman.Properties.getIntProperty("enableGPSTimeout" + mode, 0) > 0)
                        {
                            watcher = new Geolocator()
                            {
                                MovementThreshold = _avgSpeed,
                                DesiredAccuracy = PositionAccuracy.High,
                                ReportInterval = (uint)(nowhereman.Properties.getIntProperty("enableGPSTimeout" + mode, 0) * 1000)
                            };
                        }
                        else if (intelligence)
                        {
                            watcher = new Geolocator()
                            {
                                //MovementThreshold = _avgSpeed,
                                DesiredAccuracy = PositionAccuracy.High,
                                ReportInterval = (uint)1000
                            };
                        }
                        else
                        {
                            watcher = new Geolocator()
                            {
                                MovementThreshold = _avgSpeed,
                                DesiredAccuracy = PositionAccuracy.High
                            };
                        }
                    }

                    watcher.StatusChanged += watcher_StatusChanged;
                    watcher.PositionChanged += watcher_PositionChanged;

                    StartLocationExtensionSession();
                }
                catch (Exception e)
                {
                    MessageDialog dialog = new MessageDialog("Attention GPS not available because " + e.Message, resourceLoader.GetString("Attention"));
                    await dialog.ShowAsync();

                    watcher = null;
                    hasGPSEnabled = false;
                }
            }
            else
            {
                watcher = null;
            }

            counter = 0;

            tokenSource = new CancellationTokenSource();

            monitor = Task.Factory.StartNew(() => _monitor(), tokenSource.Token);

            timer = new Timer(new TimerCallback(this.OnTimedEvent), new AutoResetEvent(false), 0, REFRESH_EVERY);

            if (intelligence)
            {
                timerIntelligence = new Timer(new TimerCallback(this.NEWdetectMovement), new AutoResetEvent(false), 1000, 1000);

            }

            if (storeAllData)
            {
                string name = "debug_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".txt";
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (store.FileExists(name))
                        store.DeleteFile(name);

                    isoFileStream = new StreamWriter(new IsolatedStorageFileStream(name, FileMode.OpenOrCreate, store));
                }
            }



            return idSession;
        }

        //public void changePace(double distance, TimeSpan t)
        //{
        //    PACEstartTime = DateTime.Now.Ticks;
        //    PACEcurrentDistance = 0;

        //    //PACEobjectiveTime = t;
        //    //PACEobjectiveDistance = distance;

        //    double totalSeconds = t.TotalSeconds;
        //    ObjPace = totalSeconds / distance;

        //    hasObjective = true;
        //}

        public void changePace(double pace)
        {
            hasObjective = false; // Set to false until all data has been changed

            PACEstartTime = DateTime.Now.Ticks;
            PACEcurrentDistance = 0;

            ObjPace = pace;
            objectiveSet = false;
            adviceNumber = 0;

            hasObjective = true;
        }

        public void reallyStart()
        {
            if (movements != null)
                movements.clear();
            lastPositionMovement = null;
            lastUnknown = 0;

            running = true;
            startTime = DateTime.Now.Ticks;
            //despTime = 0;
            currentTime = 0;
            currentDistance = despDistance;

            PACEcurrentDistance = 0;
            PACEstartTime = DateTime.Now.Ticks;

            initialPoint = null;
            previousPoint = null;
            //timeToEnd = 0;

            pauseTime = 0;
            pauseDistance = 0;
            pausedPosition = null;

            dataCollection = true;

            waitToStart = false;
            starting = false;

            wasStarted = true;
            counter = 0;

            lastReceived = null;
            //reallyLastReceived = null;

        }

        private void resetCmds()
        {
            lock (_locker)
            {
                command.Clear();
            }
        }

        public void addCmds(object r)
        {
            bool N = false;
            lock (_locker)
            {
                N = command.Count <= 0;
                command.Enqueue(r);
            }

            if (N)
            {
                lock (_locker)
                {
                    Monitor.Pulse(_locker);
                }
            }
        }

        private void _monitor()
        {
            try
            {
#if(DEBUG)
                System.Diagnostics.Debug.WriteLine("Starting MONITOR");
#endif
                while (!tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        object cmd = null;
                        lock (_locker)
                        {
                            if (command.Count > 0)
                            {
                                cmd = command.Dequeue();
                            }
                        }

#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("MONITOR action " + cmd);
#endif

                        if (cmd == null)
                        {
                            lock (_locker)
                            {
                                Monitor.Wait(_locker);
                            }
                            continue;
                        }

                        if (cmd is _TriggerAction)
                        {
                            _TriggerAction _cmd = cmd as _TriggerAction;
                            if (_cmd.question)
                            {
                                processpopup(_cmd);
                            }
                            else
                            {
                                if (null != handler) handler(this, _cmd);
                            }
                        }
                        else if (cmd is string)
                        {
                            string s = (cmd as string);

                            if (s.Equals("REALLY_STOP"))
                            {
                                break;
                            }
                            else
                            {
                                if (null != handlerA) handlerA(this, s);
                            }
                        }
                    }
                    catch (Exception ee)
                    {
#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("Error MONITOR " + ee.ToString());
#endif
                        nowhereman.LittleWatson.instance.Error("MONITOR", ee);
                    }
                }
            }
            finally
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("Ending MONITOR");
#endif
            }
        }

        private double lastTimerDistance = 0;
        //        private bool detectedPause = false, detectedRunning = false;

        //private bool ignorePause = false;
        //private bool ignoreContinue = false;

        private _TriggerAction questionAction = null;
        public readonly System.Object lockPointPopup = new System.Object();

        private void processpopup(_TriggerAction _action)
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("processpopup " + _action.action + " " + _action.cancels);
#endif
            lock (lockPointPopup)
            {
                if (questionAction != null)
                {
                    if (questionAction.action.Equals(_action.action))
                    {
                    }
                    else if (questionAction.action.Equals(_action.cancels))
                    {
                        questionAction = null;

                        if (null != popup) popup(this, _action, false);
                    }


                    // TODO it cannot process several popups at same time
                    return;
                }

                if ((Constants.DO_PAUSE.Equals(_action.action) || Constants.CHANGE_PACE.Equals(_action.action)) && paused)
                {
                    return;
                }

                if (Constants.DO_CONTINUE.Equals(_action.action) && !paused)
                {
                    return;
                }

                questionAction = _action;

                if (wasObscured)
                {
                    questionAction.question = false;
                    addCmds(questionAction);

                    questionAction = null;
                }
                else
                {
                    if (null != popup) popup(this, _action, true);
                }
            }
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("END processpopup " + _action.action + " " + _action.cancels);
#endif
        }

        public void cancelPopup()
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("cancelPopup ");
#endif
            lock (lockPointPopup)
            {
                if (null != popup) popup(this, questionAction, false);

                if (questionAction != null)
                {
                    if (Constants.DO_PAUSE == questionAction.action)
                    {
                        movements.clear();
                        lastPositionMovement = null;
                        lastUnknown = 0;
                    }
                    else if (Constants.DO_CONTINUE == questionAction.action)
                    {
                        movements.clear();
                        lastPositionMovement = null;
                        lastUnknown = 0;
                    }

                    questionAction = null;
                }
            }
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("END cancelPopup ");
#endif
        }

        private IDictionary<string, string> ignoreList = new Dictionary<string, string>();

        internal bool isIgnored(_TriggerAction _action)
        {
            return isIgnored(_action.action);
        }

        internal bool isIgnored(string name)
        {
            return ignoreList.ContainsKey(name);
        }

        public void ignorePopup()
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("ignorePopup ");
#endif
            lock (lockPointPopup)
            {
                if (null != popup) popup(this, questionAction, false);

                if (questionAction != null)
                {
                    _TriggerAction old = questionAction;

                    if (old != null && old.action != null)
                    {
                        if (!ignoreList.ContainsKey(old.action))
                        {
                            ignoreList.Add(old.action, old.action);
                        }
                    }

                    questionAction = null;
                }
            }
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("END ignorePopup ");
#endif
        }

        public void acceptPopup()
        {
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("acceptPopup ");
#endif
            lock (lockPointPopup)
            {
                if (null != popup) popup(this, questionAction, false);

                if (questionAction != null)
                {
                    questionAction.question = false;
                    addCmds(questionAction);

                    questionAction = null;
                }
            }
#if(DEBUG)
            System.Diagnostics.Debug.WriteLine("END acceptPopup ");
#endif
        }

        private Geocoordinate lastPositionMovement = null;
        private int lastUnknown = 0;
        private double lastDistance = 0;
        private bool hasLost = false;

        private void NEWdetectMovement(object source) //double _currentDistance, bool toStop, double _accuracy)
        {
            //System.Diagnostics.Debug.WriteLine("TICK");
            if (dataLost)
            {
                hasLost = true;

                lock (lockPointPopup)
                {
                    if (questionAction != null)
                    {
                        if (Constants.DO_PAUSE.Equals(questionAction.action) || Constants.CHANGE_PACE.Equals(questionAction.action) ||
                            Constants.DO_CONTINUE.Equals(questionAction.action))
                        {
                            if (null != popup) popup(this, questionAction, false);
                            questionAction = null;
                        }
                    }

                }

                return;
            }

            if (hasLost)
            {
                hasLost = false;
                if (movements != null)
                    movements.clear();
                lastPositionMovement = null;
                lastUnknown = 0;
            }

            try
            {
                Geocoordinate coord = null, nextCoord = null;
                int Num = lastPositions.getCount();
                int Cou = Num;
                int N = movements.getN();
                int action = -1;
                double distMax = 0;

                if (Cou > 0)
                {
                    while (Num-- > 0)
                    {
                        coord = null;
                        try
                        {
                            coord = lastPositions.dequeue();

                        }
                        catch (Exception ex)
                        {
                            //System.Diagnostics.Debug.WriteLine("Error " + ex.Message);
                            break;
                        }

                        if (coord != null)
                        {
                            if (lastPositionMovement != null)
                            {
                                double dist = Math.Round(lastPositionMovement.GetDistanceTo(coord), 1);
                                double error = Math.Round(lastPositionMovement.Accuracy + coord.Accuracy, 1);
#if(DEBUG)
                                System.Diagnostics.Debug.WriteLine("CHECKING " + dist.ToString("0.0", CultureInfo.InvariantCulture) + " " + error.ToString("0.0", CultureInfo.InvariantCulture) + " (" + expectedDistance.ToString("0.0", CultureInfo.InvariantCulture) + ")" + " " + coord.Speed?.ToString("0.0", CultureInfo.InvariantCulture));
#endif
                                // coord.IsUnknown not necessary because it's already checked
                                if (/*coord.Speed > 0 &&*/ dist >= error)
                                {
                                    action = 1;

                                    if (nextCoord == null)
                                    {
                                        nextCoord = coord;
                                    }
                                    else if (coord.Accuracy < nextCoord.Accuracy)
                                    {
                                        nextCoord = coord;
                                    }

                                    //System.Diagnostics.Debug.WriteLine("detected");
                                }
                                else
                                {
                                    if (dist > 0 && error > expectedDistance)
                                    {
                                        // unknown
                                    }
                                    else
                                    {
                                        if (action != 1)
                                        {
                                            action = 0;
                                        }
                                    }
                                }

                                distMax = Math.Max(distMax, dist);
                            }
                            else
                            {

                                if (nextCoord == null)
                                {
                                    nextCoord = coord;
                                }
                                else if (coord.Accuracy < nextCoord.Accuracy)
                                {
                                    nextCoord = coord;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if ((action == 0 || action == -1) && distMax > 0)
                    {
                        if (lastDistance < distMax)
                        {
                            action = 1;
#if(DEBUG)
                            System.Diagnostics.Debug.WriteLine("TO RUN!");
#endif

                        }
                        else if (lastDistance > distMax)
                        {
                            action = 0;
#if(DEBUG)
                            System.Diagnostics.Debug.WriteLine("TO STOP!");
#endif
                        }
                        else
                        {
                            action = 0;
#if(DEBUG)
                            System.Diagnostics.Debug.WriteLine("TO STOP!");
#endif
                        }
                    }

                    if (nextCoord != null)
                    {
                        lastPositionMovement = nextCoord;
                        lastDistance = 0;
                    }
                    else
                    {
                        lastDistance = distMax;
                    }

                }
#if(DEBUG)
                System.Diagnostics.Debug.WriteLine("a=" + action + " uu=" + lastUnknown + " cou=" + Cou + " N=" + N + " " + (lastPositionMovement != null) + " " + ((lastPositionMovement != null ? lastPositionMovement.Speed : -1)));
#endif
                if (action == -1)
                {
                    lastUnknown++;
                    //// if all pending are unknown -> stoppped
                    if (lastUnknown >= N)
                    {
                        while (--lastUnknown >= 0)
                        {
                            movements.Add(-1);
                        }
                        lastUnknown = 0;
                    }
                }
                else
                {
                    // after several unknown! one know means same
                    while (--lastUnknown >= 0)
                    {
                        movements.Add(action);
                    }
                    lastUnknown = 0;
                    movements.Add(action);
                }

                bool triggerPause = false;
                bool triggerRun = false;

                int R = movements.getRunning(), S = movements.getStopping(), U = movements.getUnknow() + lastUnknown, C = movements.getCount();

                if (N == C)
                {
                    //if ((S > R && (S + U) == 0) || (S <= R && S == 0)) //(2 * R > 3 * (S + U) && (S + U) > 0) || 3 * R >= N)
                    if (2 * 2 * R > 5 * (2 * S + U))
                    {
                        triggerRun = true;
                    }
                    //else if ((S >= R && R == 0) || (S < R && (R + U) == 0)) /// (2 * (S + U) > 3 * R && R > 0) || 3 * (S + U) >= N)
                    else if (2 * (2 * S + U) > 5 * 2 * R)
                    {
                        triggerPause = true;
                    }
                }

                if (notifyMovement != null)
                {
                    notifyMovement(this, S, U, R);
                }

                if ((!waitToStart) && triggerPause)
                {
                    if (!ignoreList.ContainsKey(Constants.DO_PAUSE)) // ignorePause) // && !detectedPause)
                    {
#if(DEBUG)
                        System.Diagnostics.Debug.WriteLine("DO PAUSE!");
#endif
                        TriggerActionNumber aa = (new TriggerActionNumber(this) { action = Constants.DO_PAUSE, num = 0, question = true, cancels = Constants.DO_CONTINUE });

                        processpopup(aa);
                    }
                }
                else if (triggerRun)
                {
                    if (waitToStart)
                    {
#if(DEBUG)
                        System.Diagnostics.Debug.WriteLine("DO RUN!");
#endif
                        addCmds(new TriggerActionNumber(this) { action = Constants.DO_START, num = 0, question = false, cancels = Constants.DO_PAUSE });

                    }
                    else if (!ignoreList.ContainsKey(Constants.DO_CONTINUE)) // && !detectedRunning)
                    {
#if(DEBUG)
                        System.Diagnostics.Debug.WriteLine("DO RUN!");
#endif
                        TriggerActionNumber aa = (new TriggerActionNumber(this) { action = Constants.DO_CONTINUE, num = 0, question = true, cancels = Constants.DO_PAUSE });

                        processpopup(aa);
                    }
                }

#if(DEBUG)
                System.Diagnostics.Debug.WriteLine("RUNN detectMovement R=" + R + " S=" + (S + U) + " N=" + N + " " + triggerPause + " " + triggerRun + " " + HorAccuracy);
#endif
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("detectMOvement", ex);
            }
        }

        private void OnTimedEvent(object source) //, ElapsedEventArgs e)
        {
            try
            {
                double sp = Double.MaxValue;

                if (hasGPSEnabled)
                {
                    long la = lastPositionTime;
                    long na = DateTime.Now.Ticks;

                    sp = la > 0 ? Math.Round(new TimeSpan(na - la).TotalSeconds, 1) : Double.MaxValue;
                    if (requiredAccuracyTime)
                    {
                        if (sp != previousAcuraccy)
                        {
                            if (null != handlerS) handlerS(this, null, sp, null);
                            if (sp > MAX_ACCURACY || HorAccuracy > MAX_ACCURACY)
                            {
                                dataHasBeenLost(true, true);
                            }
                            else if (sp <= requiredAccuracy)
                            {
                                dataHasBeenLost(false, false);
                            }
                            else
                            {
                                dataHasBeenLost(true, false);
                            }
                            previousAcuraccy = sp;
                        }
                    }
                }

                if (starting)
                {
                    if (lastReceived != null)
                        if (null != handlerP) handlerP(this, new TimeSpan(currentTime), lastReceived.Speed.HasValue ? lastReceived.Speed.Value : double.NaN, lastReceived.Point.Position.Altitude, 0, 0.0, 0.0, HorAccuracy, 0, lastReceived);

                    if (requiredAccuracyTime)
                    {
                        if (sp <= requiredAccuracy && HorAccuracy <= MAX_ACCURACY)
                        {
                            starting = false;

                            addCmds(Constants.READY_TO_START_ACTION);
                        }
                    }
                }
                else if (waitToStart)
                {
                    //if (counter % MOVEMENT_REFRESH == 0)
                    //{
                    //    detectMovement(0, true, HorAccuracy);
                    //}
                    if ((counter++) % DELAY_REFRESH == 0)
                    {
                        if (lastReceived != null)
                            if (null != handlerP) handlerP(this, new TimeSpan(currentTime), lastReceived.Speed.HasValue ? lastReceived.Speed.Value : double.NaN, lastReceived.Point.Position.Altitude, 0, 0.0, 0.0, HorAccuracy, 0, lastReceived);
                    }
                }
                else
                {
                    long currentTick = DateTime.Now.Ticks;
                    currentTime = currentTick - startTime - despTime;
                    TimeSpan currentTimeObj = TimeSpan.FromTicks(currentTime);
                    var T = lastReceived;

                    long PACEcurrentTime = currentTick - PACEstartTime;
                    TimeSpan PACEcurrentTimeObj = TimeSpan.FromTicks(PACEcurrentTime);

                    if (pints.getCount() > 0)
                    {
                        double _currentSpeed = 0, _currentAltitude = 0, _currentDistance = 0, _ascendent = 0, _descendent = 0, _accuracy;

                        InmediatePoint intp = pints.getLast();

                        _currentSpeed = intp.sumSpeedTmp > 0 ? intp.NunitS / intp.sumSpeedTmp : 0;
                        _currentAltitude = intp.sumAltitudeTmp != 0 ? intp.NunitA / intp.sumAltitudeTmp : 0;

                        _currentDistance = intp.Distance;
                        _ascendent = intp.Ascendent;
                        _descendent = intp.Descendent;
                        _accuracy = intp.Accuracy;

                        if ((counter++) % DELAY_REFRESH == 0)
                        {
                            if (null != handlerP) handlerP(this, currentTimeObj, _currentSpeed, _currentAltitude, _currentDistance, _ascendent, _descendent
                                                                 , distanceFromStart.getCount() > 0 ? distanceFromStart.getLast() : Double.NaN
                                                                 , distanceToEnd.getCount() > 0 ? distanceToEnd.getLast() : Double.NaN, T);
                        }
                    }
                    else
                    {
                        if ((counter++) % DELAY_REFRESH == 0)
                        {
                            if (null != handlerP) handlerP(this, currentTimeObj, lastReceived != null ? (lastReceived.Speed.HasValue ? lastReceived.Speed.Value : double.NaN) : 0, lastReceived != null ? lastReceived.Point.Position.Altitude : 0, 0, 0, 0
                                                                 , distanceFromStart.getCount() > 0 ? distanceFromStart.getLast() : Double.NaN
                                                                 , distanceToEnd.getCount() > 0 ? distanceToEnd.getLast() : Double.NaN, T);
                        }
                    }
                    if (!paused)
                    {
                        bool changedPointTime = false;
                        while (timeActionPoint < timeActions.Count && currentTime >= timeActions[timeActionPoint + 1].time.Ticks)
                        {
                            timeActionPoint++;
                            changedPointTime = currentTime > despTime;
                        }

                        TriggerActionTime tmpT = null;

                        if (changedPointTime)
                        {
                            tmpT = timeActions[timeActionPoint];

                            if (tmpT.action.Equals(Constants.MINUTES_FROM))
                            {
                                TimeSpan tmp = tmpT.time;

                                tmp = tmp.Add(new TimeSpan(0, intervalTime, 0));
                                if (objectiveTime == TimeSpan.MinValue)
                                {
                                    timeActions.Add(new TriggerActionTime(this) { action = Constants.MINUTES_FROM, time = tmp, param = null });
                                }
                                else
                                {
                                    timeActions.Add(new TriggerActionTime(this) { action = Constants.MINUTES_FROM, time = tmp, param = objectiveTime });
                                }
                            }
                        }

                        if (changedPointTime)
                        {
                            addCmds(tmpT);
                        }

                        if (!dataLost && hasObjective)
                        {
                            if (PACEcurrentTimeObj.TotalMinutes >= MIN_MINUTE)
                            {
                                double _pace = Utils.toPace(PACEcurrentTimeObj, PACEcurrentDistance);

                                //double seconds = PACEcurrentTimeObj.TotalSeconds;

                                //// Negative -> got rthytm
                                //double res = 1 - ObjPace * (PACEcurrentDistance / seconds);

                                //double totalSeconds = PACEobjectiveTime.TotalSeconds;
                                //double res = 1 - (PACEcurrentDistance * totalSeconds) / (PACEobjectiveDistance * seconds);
                                int signRes = 0;

                                if (Math.Round(ObjPace, 2) == Math.Round(_pace, 2))
                                {
                                    signRes = 0;
                                }
                                else
                                {
                                    if (Math.Round(ObjPace, 1) == Math.Round(_pace, 1))
                                    {
                                        signRes = Math.Sign(_pace - ObjPace);
                                    }
                                    else
                                    {
                                        signRes = 2 * Math.Sign(_pace - ObjPace);
                                    }
                                }


                                //if (Math.Abs(res) >= LIMIT)
                                //    signRes = Math.Sign(res) * 2;
                                //else
                                //    signRes = Math.Sign(res);

                                if (signRes != previousRes)
                                {
                                    TriggerActionNumber tm;
                                    if (signRes > 0)
                                    {
                                        tm = new TriggerActionNumber(this) { action = Constants.SHOW_INFO_YOU_LOST_THE_RHYTHM, num = Math.Abs(signRes) };
                                    }
                                    else
                                    {
                                        tm = new TriggerActionNumber(this) { action = Constants.SHOW_INFO_YOU_GO_TO_FAST, num = Math.Abs(signRes) };
                                    }
                                    addCmds(tm);
                                    //handlerR(-signRes);
                                    previousRes = signRes;
                                }

                                TimeSpan lastT = TimeSpan.FromTicks(currentTime - lastAdvice);
                                if (objectiveSet)
                                {
                                    if (Math.Abs(signRes) >= 2)
                                    {
                                        if (lastT.TotalMinutes >= MIN_MINUTE) // TODO minutes depending on speed ???
                                        {
                                            TriggerActionNumber tm;

                                            lastAdvice = currentTime;

                                            if (signRes > 0)
                                            {
                                                adviceNumber--;
                                                if (adviceNumber < 0 && (-adviceNumber) > limitAdviceNumber)
                                                {
                                                    if (intelligencePace)
                                                    {
                                                        double tochange = Utils.toPace(PACEcurrentTime, PACEcurrentDistance) + 0.1;

                                                        if (tochange > 0)
                                                        {
                                                            tm = new TriggerActionNumber(this) { action = Constants.CHANGE_PACE, param = tochange, question = true };
                                                        }
                                                        else
                                                        {
                                                            tm = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tm = new TriggerActionNumber(this) { action = Constants.YOU_COMPLETE_LOST_THE_RHYTHM, num = Math.Abs(signRes) };
                                                    }
                                                }
                                                else
                                                {
                                                    tm = new TriggerActionNumber(this) { action = Constants.YOU_LOST_THE_RHYTHM, num = Math.Abs(signRes) };
                                                }
                                            }
                                            else
                                            {
                                                adviceNumber++;

                                                if (adviceNumber > limitAdviceNumber)
                                                {
                                                    double tochange = Utils.toPace(PACEcurrentTime, PACEcurrentDistance) - 0.1;

                                                    if (intelligencePace)
                                                    {
                                                        if (tochange > 0)
                                                        {
                                                            tm = new TriggerActionNumber(this) { action = Constants.CHANGE_PACE, param = tochange, question = true };
                                                        }
                                                        else
                                                        {
                                                            tm = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        tm = new TriggerActionNumber(this) { action = Constants.YOU_GO_TO_FAST, num = Math.Abs(signRes) };
                                                    }
                                                }
                                                else
                                                {
                                                    tm = new TriggerActionNumber(this) { action = Constants.YOU_GO_TO_FAST, num = Math.Abs(signRes) };
                                                }
                                            }

                                            if (adviceNumber < 0 && (-adviceNumber) > limitAdviceNumber)
                                            {
                                                objectiveSet = false;
                                            }

                                            if (tm != null)
                                            {
                                                addCmds(tm);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        adviceNumber = 0;
                                    }
                                }
                                else
                                {
                                    // not objective set

                                    if (Math.Abs(signRes) < 2 || signRes < 0) // TODO depending on speed???
                                    {
                                        objectiveSet = true;

                                        TriggerActionNumber tm = new TriggerActionNumber(this) { action = Constants.YOU_GOT_THE_RHYTHM, num = 0 };
                                        addCmds(tm);

                                        lastAdvice = currentTime;
                                        adviceNumber = 0;

                                        //storeExtra(tm, currentDistance, currentTime, T);
                                    }
                                    else
                                    {
                                        if (intelligencePace)
                                        {
                                            if (lastT.TotalMinutes >= MIN_MINUTE) // TODO minutes depending on speed ???
                                            {
                                                lastAdvice = currentTime;

                                                adviceNumber--;
                                                if (adviceNumber < 0 && (-adviceNumber) > limitAdviceNumber)
                                                {
                                                    double tochange = Utils.toPace(PACEcurrentTime, PACEcurrentDistance) - 0.1;

                                                    if (tochange > 0)
                                                    {
                                                        TriggerActionNumber tm = new TriggerActionNumber(this) { action = Constants.CHANGE_PACE, param = tochange, question = true };
                                                        addCmds(tm);
                                                    }
                                                    lastAdvice = currentTime;
                                                    adviceNumber = 0;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("error on timer ", ex);
            }
        }

        private void loopDetected(double currentDistance, long currentTime, Geocoordinate T)
        {
            loopCount++;

            LOOPcurrentDistance.Add(currentDistance);
            LOOPstartTime.Add(currentTime);

            TriggerActionNumber tm = new TriggerActionNumber(this) { action = Constants.LOOP_DETECTED + "_" + loopCount.ToString(), num = loopCount };
            storeExtra(tm, currentDistance, currentTime, T);

            addCmds(tm);
        }

        private void storeExtra(TriggerActionNumber tm, double currentDistance, long currentTime, Geocoordinate T)
        {
            DataBaseManager.instance.InsertPoint(sessionData, tm.action, T, currentDistance, currentTime, Constants.EXTRA_POINTS);
        }

        public void pause(bool user)
        {
            if (!paused)
            {
                paused = true;
                if (user)
                {
                    movements.clear();
                    lastPositionMovement = null;
                    lastUnknown = 0;
                }

                pauseTime = DateTime.Now.Ticks;
                pauseDistance = currentDistance;
                pausedPosition = previousPoint;

                addCmds(Constants.PAUSE_ACTION);
            }
        }

        public void resume(bool user)
        {
            if (paused)
            {
                if (user)
                {
                    movements.clear();
                    lastPositionMovement = null;
                    lastUnknown = 0;
                }
                despTime += DateTime.Now.Ticks - pauseTime;

                currentDistance = pauseDistance;
                previousPoint = pausedPosition;

                PACEcurrentDistance = 0;
                PACEstartTime = DateTime.Now.Ticks;

                lastPositionTime = -1;
                restart = true;
                paused = false;
                counter = 0;

                // enable rhythm detection from start
                objectiveSet = false;


                addCmds(Constants.RESUME_ACTION);
            }
        }

        public void stop()
        {
            addCmds(Constants.STOP_ACTION);
        }

        public async void _stop(bool inter)
        {
            if (movements != null) movements.clear();
            lastPositionMovement = null;
            lastUnknown = 0;

            try
            {
                running = false;


                try
                {
                    if (isoFileStream != null)
                    {
                        isoFileStream.Flush();
                        isoFileStream.Dispose();
                        isoFileStream = null;
                    }
                }
                catch (Exception ex)
                {
                    nowhereman.LittleWatson.instance.Error("Error debug", ex);
                }

                try
                {
                    if (timerIntelligence != null)
                    {
                        timerIntelligence.Dispose();
                        timerIntelligence = null;
                    }
                }
                catch (Exception e)
                {
                    nowhereman.LittleWatson.instance.Error("stop timerIntelligence", e);
                }

                try
                {
                    if (timer != null)
                    {
                        timer.Dispose();
                        timer = null;
                    }
                }
                catch (Exception e)
                {
                    nowhereman.LittleWatson.instance.Error("stop timer", e);
                }

                try
                {
                    if (watcher != null)
                    {
                        watcher.PositionChanged -= watcher_PositionChanged;
                        watcher.StatusChanged -= watcher_StatusChanged;
                        watcher = null;
                    }
                }
                catch (Exception e)
                {
                    nowhereman.LittleWatson.instance.Error("stop watcher", e);
                }

                try
                {
                    if (tokenSource != null)
                    {
                        tokenSource.Cancel();

                        addCmds("REALLY_STOP"); // To wake up thread an close thread

                    }
                }
                catch (Exception e)
                {
                    nowhereman.LittleWatson.instance.Error("stop bw", e);
                }

                started = false;

                bool remove = false;

                if (inter)
                {
                    if (!starting && !waitToStart && (hasGPSEnabled && (!receivedData || currentDistance <= 0)))
                    {
                        if (!isNew)
                        {
                            remove = true;
                        }
                        else
                        {
                            //MessageDialog dialog = new MessageDialog(resourceLoader.GetString("SessionWithNoData"), resourceLoader.GetString("Attention"));

                            //dialog.Commands.Add(new UICommand(resourceLoader.GetString("Cancel"), null, "resume"));
                            //dialog.Commands.Add(new UICommand(resourceLoader.GetString("Ok"), null, "delete"));
                            //dialog.CancelCommandIndex = 0;
                            //dialog.DefaultCommandIndex = 0;
                            //IUICommand selected = await dialog.ShowAsync();
                            //if (selected.Id is string && (string)selected.Id == "delete")
                            //{
                            remove = true;
                            //}
                        }
                    }
                }

                if (!wasStarted || starting || waitToStart || remove)
                {
                    wasStarted = false;
                    if (isNew)
                    {
                        sessionData.IdPath = path.Id;
                        DataBaseManager.instance.DeleteSession(sessionData, true);
                    }
                }
                else
                {
                    if (paused)
                    {
                        despTime += DateTime.Now.Ticks - pauseTime;
                        long currentTick = DateTime.Now.Ticks;
                        currentTime = currentTick - startTime - despTime;
                        currentDistance = pauseDistance;
                    }

                    if (isNew)
                    {
                        sessionData.Ascendent = ascendent;
                        sessionData.Descendent = descendent;

                        sessionData.AvgPace = Utils.toPace(currentTime, currentDistance);

                        if (receivedData)
                        {
                            if (N > 0)
                            {
                                sessionData.CenterLat = centerLat / N;
                                sessionData.CenterLon = centerLon / N;
                                sessionData.MaxLat = maxLat;
                                sessionData.MaxLon = maxLon;
                                sessionData.MinLat = minLat;
                                sessionData.MinLon = minLon;

                                sessionData.Distance = currentDistance;
                                if (lastReceived != null)
                                {
                                    sessionData.EndLat = lastReceived.Point.Position.Latitude;
                                    sessionData.EndLon = lastReceived.Point.Position.Longitude;
                                }
                                //if (initialPoint != null)
                                //{
                                //    sessionData.startLat = initialPoint.Latitude;
                                //    sessionData.startLon = initialPoint.Longitude;
                                //}
                            }
                            if (Ns > 0 && avgSpeed != 0)
                            {
                                sessionData.AvgSpeed = Ns / avgSpeed;

                                sessionData.MinSpeed = minSpeed;
                                sessionData.MaxSpeed = maxSpeed;
                            }
                            else
                            {
                                sessionData.AvgSpeed = 0;

                                sessionData.MinSpeed = 0;
                                sessionData.MaxSpeed = 0;
                            }

                            if (avgAltitude != 0 && Na > 0)
                            {
                                sessionData.AvgAltitude = Na / avgAltitude;
                                sessionData.MaxAltitude = maxAltitude;
                                sessionData.MinAltitude = minAltitude;
                            }
                            else
                            {
                                sessionData.AvgAltitude = 0;
                                sessionData.MaxAltitude = 0;
                                sessionData.MinAltitude = 0;
                            }


                        }
                        else
                        {
                            sessionData.CenterLat = null;
                            sessionData.CenterLon = null;

                            sessionData.AvgSpeed = null;
                            sessionData.AvgAltitude = null;

                            sessionData.MaxAltitude = null;
                            sessionData.MinAltitude = null;

                            sessionData.MaxLat = null;
                            sessionData.MaxLon = null;
                            sessionData.MinLat = null;
                            sessionData.MinLon = null;

                            sessionData.MinSpeed = null;
                            sessionData.MaxSpeed = null;

                            sessionData.Distance = null;
                            sessionData.EndLat = null;
                            sessionData.EndLon = null;
                            sessionData.StartLat = null;
                            sessionData.StartLon = null;
                        }
                        sessionData.Duration = currentTime;
                    }
                    else
                    {
                        sessionData.Ascendent += ascendent;
                        sessionData.Descendent += descendent;

                        if (receivedData)
                        {
                            if (currentDistance > 0)
                            {
                                sessionData.AvgPace = (sessionData.AvgPace + Utils.toPace(currentTime, currentDistance)) / 2;
                            }

                            if (N > 0)
                            {
                                sessionData.CenterLat = centerLat / N; // TODO avg
                                sessionData.CenterLon = centerLon / N; // TODO avg                  
                                sessionData.MaxLat = Math.Max(maxLat, sessionData.MaxLat.Value);
                                sessionData.MaxLon = Math.Max(maxLon, sessionData.MaxLon.Value);
                                sessionData.MinLat = Math.Min(minLat, sessionData.MinLat.Value);
                                sessionData.MinLon = Math.Min(minLon, sessionData.MinLon.Value);
                                sessionData.MinSpeed = Math.Min(minSpeed, sessionData.MinSpeed.Value);
                                sessionData.MaxSpeed = Math.Max(maxSpeed, sessionData.MaxSpeed.Value);

                                sessionData.Distance = sessionData.Distance.Value + currentDistance;
                                if (lastReceived != null)
                                {
                                    sessionData.EndLat = lastReceived.Point.Position.Latitude;
                                    sessionData.EndLon = lastReceived.Point.Position.Longitude;
                                }
                            }

                            if (Ns > 0 && avgSpeed != 0)
                            {
                                sessionData.AvgSpeed = (sessionData.AvgSpeed + Ns / avgSpeed) / 2;
                            }

                            if (avgAltitude != 0 && Na > 0)
                            {
                                sessionData.AvgAltitude = (sessionData.AvgAltitude + Na / avgAltitude) / 2;
                                sessionData.MaxAltitude = Math.Max(sessionData.MaxAltitude.Value, maxAltitude);
                                sessionData.MinAltitude = Math.Min(sessionData.MinAltitude.Value, minAltitude);

                            }

                            //sessionData.startLat = startLatitude;
                            //sessionData.startLon = startLongitude;

                        }

                        sessionData.Duration = sessionData.Duration.Value + currentTime;
                    }

                    DataBaseManager.instance.UpdateSession(sessionData);

                    if (higherPoint != null) DataBaseManager.instance.InsertPoint(sessionData, Constants.HIGHER, higherPoint, higherPointDistance, higherPointTime, Constants.HIGHER);
                    if (lowerPoint != null) DataBaseManager.instance.InsertPoint(sessionData, Constants.LOWER, lowerPoint, lowerPointDistance, lowerPointTime, Constants.LOWER);
                }
            }
            catch (Exception e)
            {
                nowhereman.LittleWatson.instance.Error("stop", e);
            }
        }

#if(DEBUGx)
        private void watcher_StatusChanged(PositionStatus pos)
        {
#else
        private void watcher_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            PositionStatus pos = args.Status;
#endif

#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("?");
#endif
            try
            {
                bool _dataLost = false;
                switch (pos)
                {
                    case PositionStatus.Disabled:
                    case PositionStatus.NotAvailable:
                    case PositionStatus.NotInitialized:
                        if (null != handlerS) handlerS(this, "KO", null, null);
                        _dataLost = true;
                        break;
                    case PositionStatus.NoData:
                        if (null != handlerS) handlerS(this, "NO DATA", null, null);
                        _dataLost = true;
                        break;
                    case PositionStatus.Initializing:
                        if (null != handlerS) handlerS(this, "WAIT", null, null);
                        _dataLost = true;
                        break;
                    default:
                        if (null != handlerS) handlerS(this, "OK", null, null);
                        _dataLost = false;
                        break;
                }

                dataHasBeenLost(_dataLost, true);

            }
            catch (Exception e)
            {
                nowhereman.LittleWatson.instance.Error("watcher_StatusChanged", e);
            }
        }

        private void dataHasBeenLost(bool _dataLost, bool complete)
        {
            if (_dataLost)
            {
                if (starting)
                {
                    // while starting doesn't matter if data is lost or not
                }
                else if (waitToStart)
                {
                    // data lost while waiting to start, restart again
                    starting = true;
                }
                else if (!paused)
                {
                    if (_dataLost != dataLost)
                    {
                        // we should pause data collection to avoid invalid data
                        actionMsg = "-"; // SIGNAL_LOST

                        if (complete)
                        {
                            addCmds(new TriggerActionNumber(this) { action = Constants.SIGNAL_IS_LOST, num = 0 });
                        }
                        else
                        {
                            //addCmds(new TriggerActionNumber(this) { action = Constants.LOW_SIGNAL, num = 0 });
                        }
                    }
                }
                dataCollection = false;
            }
            else if (!_dataLost)
            {
                if (starting)
                {
                    // while starting doesn't matter if data is lost or not
                }
                else if (waitToStart)
                {
                    // while wait to start do nothing
                }
                else if (!paused)
                {
                    if (_dataLost != dataLost)
                    {
                        // we should continue data collection

                        // enable loop
                        PACEcurrentDistance = 0;
                        PACEstartTime = DateTime.Now.Ticks;

                        // enable rhythm detection from start
                        objectiveSet = false;

                        actionMsg = "+";

                        movements.clear();
                        lastPositionMovement = null;
                        lastUnknown = 0;
                    }
                }
                dataCollection = true;
            }

            dataLost = _dataLost;
        }

#if(DEBUGx)
        private void watcher_PositionChanged(long utcTicks, Geocoordinate eloc)
        {
#else
        private void watcher_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            long utcTicks = args.Position.Coordinate.Timestamp.UtcTicks;
            Geocoordinate eloc = args.Position.Coordinate;
#endif

#if (DEBUG)
            System.Diagnostics.Debug.WriteLine(".");
#endif
            //if (eloc.IsUnknown)
            //{
            //    System.Diagnostics.Debug.WriteLine("POINT UNKNOWN");

            //    return;
            //}


            lastPositionTime = DateTime.Now.Ticks;

            HorAccuracy = eloc.Accuracy;
            if (Double.IsNaN(HorAccuracy)) HorAccuracy = Double.MaxValue;

            VerAccuracy = eloc.AltitudeAccuracy;
            if (!VerAccuracy.HasValue || Double.IsNaN(VerAccuracy.Value)) VerAccuracy = Double.MaxValue;

            //System.Diagnostics.Debug.WriteLine("POINT with [" + HorAccuracy + " " + VerAccuracy + "]");

            if (!requiredAccuracyTime)
            {
                if (VerAccuracy != previousVAcuraccy)
                {
                    if (null != handlerS) handlerS(this, null, null, VerAccuracy);

                    previousVAcuraccy = VerAccuracy.Value;
                }

                if (HorAccuracy != previousAcuraccy)
                {
                    if (null != handlerS) handlerS(this, null, HorAccuracy, null);
                    if (HorAccuracy > MAX_ACCURACY)
                    {
                        dataHasBeenLost(true, true);
                    }
                    else
                    {
                        dataHasBeenLost(false, false);
                    }

                    previousAcuraccy = HorAccuracy;
                }
            }

            if (starting)
            {
                if (!requiredAccuracyTime)
                {
                    if (HorAccuracy <= requiredAccuracy)
                    {
                        starting = false;

                        addCmds(Constants.READY_TO_START_ACTION);
                    }
                    else
                    {
                        lastReceived = eloc;
                    }
                }
                else
                {
                    lastReceived = eloc;
                }
            }
            else
            {
                if (paused)
                {
                    // if paused distance is related to paused Place
                    currentDistance = pauseDistance;
                    previousPoint = pausedPosition;

                    MyPositionChanged(eloc, utcTicks);
                }
                else
                {
                    if (MyPositionChanged(eloc, utcTicks))
                    {
                        previousPoint = eloc;
                    }
                }
            }
        }



        private bool MyPositionChanged(Geocoordinate L, long UtcTicks)
        {
            try
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("point " + L.ToString() + " @ " + UtcTicks.ToString());
#endif
                bool hasToStore = true;

                long __currentTime = currentTime;

                if (Double.IsNaN(L.Point.Position.Latitude) || Double.IsNaN(L.Point.Position.Longitude) || Double.IsNaN(L.Accuracy))
                {
                    return false;
                }

                if (isoFileStream != null)
                {
                    string tmp = UtcTicks.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.Point.Position.Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.Point.Position.Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.Accuracy.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.AltitudeAccuracy?.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.Point.Position.Altitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.Heading?.ToString(CultureInfo.InvariantCulture.NumberFormat) + ";" +
                        L.Speed?.ToString(CultureInfo.InvariantCulture.NumberFormat);
                    isoFileStream.WriteLineAsync(tmp);
                }

                lastPositions.Add(L);

                //reallyLastReceived = L;

                if (!storeAllPoints && !dataCollection)
                {
                    return false;
                }

                double dist = 0;
                if (previousPoint != null)
                {
                    dist = previousPoint.GetDistanceTo(L);

                    if (dist <= (previousPoint.Accuracy + L.Accuracy))
                    {
                        if (!storeAllPoints)
                        {
                            hasToStore = false;
                            //                            return false;
                        }
                        dist = 0;
                        // not valid distance! for invalid point
                    }
                    else
                    {
                        if (fixDataWithPitagoras)
                        {
                            if (previousPoint.Point.Position.Altitude > 0 && L.Point.Position.Altitude > 0 && !Double.IsNaN(previousPoint.Point.Position.Altitude) && !Double.IsNaN(L.Point.Position.Altitude))
                            {
                                double tmp = previousPoint.Point.Position.Altitude - L.Point.Position.Altitude;
                                double delta = Math.Sqrt(tmp * tmp + dist * dist);

                                dist = delta;
                            }
                        }
                    }
                }

                receivedData = true;

                long P = UtcTicks;

                if (initialPoint == null)
                {
                    initialPoint = L;

                    if (isLoop && Double.IsNaN(loopDistance) && (Double.IsNaN(endLatitude) || Double.IsNaN(endLongitude)))
                    {
                        endLatitude = L.Point.Position.Latitude;
                        endLongitude = L.Point.Position.Longitude;
                    }
                    if (!paused)
                    {
                        if (isNew)
                        {
                            sessionData.StartLat = initialPoint.Point.Position.Latitude;
                            sessionData.StartLon = initialPoint.Point.Position.Longitude;
                        }
                    }
                }

                currentDistance += dist;
                PACEcurrentDistance += dist;

                // DONE ON TOP previousPoint = L;

                if (previousP == P) P++;
                previousP = P;

                long NOW = DateTime.Now.Ticks;
                if (!paused && !waitToStart)
                {
                    bool changedPointDistance = false;
                    while (distanceActionPoint < distanceActions.Count && currentDistance >= distanceActions[distanceActionPoint + 1].distance)
                    {
                        distanceActionPoint++;
                        changedPointDistance = currentDistance > despDistance;

                        TriggerActionDistance tmpD = null;

                        if (changedPointDistance)
                        {
                            tmpD = distanceActions[distanceActionPoint];

                            if (tmpD.action.Equals(Constants.DISTANCE_FROM))
                            {
                                double tmp = tmpD.distance;

                                tmp += intervalDistance;
                                if (Double.IsNaN(objectiveDistance))
                                {
                                    distanceActions.Add(new TriggerActionDistance(this) { action = Constants.DISTANCE_FROM, distance = tmp, param = null });
                                }
                                else
                                {
                                    distanceActions.Add(new TriggerActionDistance(this) { action = Constants.DISTANCE_FROM, distance = tmp, param = objectiveDistance });
                                }
                            }
                            else if (tmpD.action.Equals(Constants.LOOP_DETECTED))
                            {
                                double tmp = tmpD.distance;
                                tmp += loopDistance;
                                tmpD.param = Utils.toPace(__currentTime - (long)tmpD.param, loopDistance);
                                tmpD.position = L;
                                distanceActions.Add(new TriggerActionDistance(this) { action = Constants.LOOP_DETECTED, distance = tmp, param = __currentTime });

                                actionMsg = "L";
                            }
                        }

                        if (changedPointDistance)
                        {
                            addCmds(tmpD);
                        }
                    }
                }
                if (!paused && !waitToStart)
                {
                    if (restart)
                    {
                        if (hasToStore)
                        {
                            restart = false;
                            //desp += e.Position.Timestamp.UtcTicks - lastReceived.Position.Timestamp.UtcTicks;
                            actionMsg = "R";
                        }
                    }
                }

                double? speed = null;
                bool isSpeedValid = false;
                if (L.Speed.HasValue && !Double.IsNaN(L.Speed.Value))
                {
                    if (L.Speed >= 0)
                    {
                        speed = L.Speed;
                        double v = speed.Value;

                        isSpeedValid = true; // speed.Value != 0;

                        minSpeed = Math.Min(minSpeed, v);
                        maxSpeed = Math.Max(maxSpeed, v);
                        if (v != 0) avgSpeed += 1 / v;
                        Ns++;
                    }
                }

                bool isAltitudeValid = false;

                double? altitude = null;
                if (!Double.IsNaN(L.Point.Position.Altitude) && L.AltitudeAccuracy.HasValue && !Double.IsNaN(L.AltitudeAccuracy.Value))
                {
                    if (L.Point.Position.Altitude < ALTITUDE_LIMIT_MIN)
                    {
                        altitude = ALTITUDE_LIMIT_MIN;
                    }
                    else if (L.Point.Position.Altitude > ALTITUDE_LIMIT)
                    {
                        altitude = ALTITUDE_LIMIT;
                    }
                    else
                    {
                        altitude = L.Point.Position.Altitude;
                    }
                    isAltitudeValid = true; // altitude.Value != 0;

                    if (maxAltitude < altitude)
                    {
                        maxAltitude = Math.Max(maxAltitude, altitude.Value);
                        higherPoint = L;
                        higherPointDistance = currentDistance;
                        higherPointTime = P;
                    }
                    if (minAltitude > altitude)
                    {
                        minAltitude = Math.Min(minAltitude, altitude.Value);
                        lowerPointDistance = currentDistance;
                        lowerPointTime = P;
                        lowerPoint = L;
                    }
                    if (altitude.Value != 0) avgAltitude += 1 / altitude.Value;
                    Na++;
                }
                if (!paused && !waitToStart)
                {
                    if (hasToStore)
                    {
                        Mesures mesure = new Mesures();
                        mesure.IdSession = sessionData.Id;
                        mesure.Latitude = L.Point.Position.Latitude;
                        mesure.Longitude = L.Point.Position.Longitude;

                        mesure.Speed = speed;
                        mesure.Altitude = altitude;

                        mesure.Id = P;
                        mesure.Action = actionMsg;


                        DataBaseManager.instance.InsertMesure(mesure);

                        actionMsg = null;
                    }
                }

                centerLat += L.Point.Position.Latitude;
                centerLon += L.Point.Position.Longitude;

                maxLat = Math.Max(maxLat, L.Point.Position.Latitude);
                minLat = Math.Min(minLat, L.Point.Position.Latitude);

                maxLon = Math.Max(maxLon, L.Point.Position.Longitude);
                minLon = Math.Min(minLon, L.Point.Position.Longitude);

                if (isAltitudeValid)
                {
                    if (previousAltitude.HasValue)
                    {
                        if (Math.Abs(previousAltitude.Value - altitude.Value) >= Math.Max(L.AltitudeAccuracy.Value, lastAltitude.AltitudeAccuracy.Value)) // TODO
                        {
                            if (previousAltitude.Value < altitude.Value)
                            {
                                ascendent += altitude.Value - previousAltitude.Value;
                            }
                            else
                            {
                                descendent += previousAltitude.Value - altitude.Value;
                            }
                            previousAltitude = altitude;
                            lastAltitude = L;
                        }
                        else
                        {
                            if (L.AltitudeAccuracy < lastAltitude.AltitudeAccuracy)
                            {
                                previousAltitude = altitude;
                                lastAltitude = L;
                            }
                        }
                    }
                    else
                    {
                        previousAltitude = altitude;
                        lastAltitude = L;
                    }
                }

                if (isSpeedValid)
                {
                    previousSpeed = speed;
                }
                N++;

                long TT = NOW / TimeSpan.TicksPerSecond;

                if (pints.getCount() <= 0)
                {
                    InmediatePoint curPoint = new InmediatePoint()
                    {
                        Time = TT,
                        Distance = currentDistance,
                        Ascendent = ascendent,
                        Descendent = descendent,
                        Accuracy = L.Accuracy
                    };

                    if (isAltitudeValid)
                    {
                        curPoint.NunitA = 1;
                        if (altitude.Value != 0)
                        {
                            curPoint.sumAltitudeTmp = 1 / altitude.Value;
                        }
                    }
                    else
                    {
                        curPoint.NunitA = 0;
                        curPoint.sumAltitudeTmp = 0;
                    }
                    if (isSpeedValid)
                    {
                        curPoint.NunitS = 1;
                        if (speed.Value != 0)
                        {
                            curPoint.sumSpeedTmp = 1 / speed.Value;
                        }
                    }
                    else
                    {
                        curPoint.NunitS = 0;
                        curPoint.sumSpeedTmp = 0;
                    }
                    pints.Add(curPoint);
                }
                else
                {
                    InmediatePoint prev = pints.getLast();
                    if (prev.Time == TT)
                    {
                        if (isSpeedValid)
                        {
                            prev.NunitS++;
                            if (speed.Value != 0)
                            {
                                prev.sumSpeedTmp += 1 / speed.Value;
                            }
                        }
                        if (isAltitudeValid)
                        {
                            prev.NunitA++;
                            if (altitude.Value != 0)
                            {
                                prev.sumAltitudeTmp += 1 / altitude.Value;
                            }
                        }
                        prev.Distance = currentDistance;
                        prev.Ascendent = ascendent;
                        prev.Descendent = descendent;
                        prev.Accuracy = Math.Max(prev.Accuracy, L.Accuracy);
                    }
                    else
                    {
                        InmediatePoint curPoint = new InmediatePoint()
                        {
                            Time = TT,
                            Distance = currentDistance,
                            Ascendent = ascendent,
                            Descendent = descendent,
                            Accuracy = L.Accuracy
                        };
                        if (isAltitudeValid)
                        {
                            curPoint.NunitA = 1;
                            if (altitude.Value != 0)
                            {
                                curPoint.sumAltitudeTmp = 1 / altitude.Value;
                            }
                        }
                        else
                        {
                            curPoint.NunitA = 0;
                            curPoint.sumAltitudeTmp = 0;
                        }
                        if (isSpeedValid)
                        {
                            curPoint.NunitS = 1;
                            if (speed.Value != 0)
                            {
                                curPoint.sumSpeedTmp = 1 / speed.Value;
                            }
                        }
                        else
                        {
                            curPoint.NunitS = 0;
                            curPoint.sumSpeedTmp = 0;
                        }
                        pints.Add(curPoint);
                    }
                }

                if (!paused && !waitToStart)
                {
                    distanceFromStart.Add(L.GetDistanceTo(initialPoint));

                    if (isLoop)
                    {
                        if (!Double.IsNaN(endLatitude) && !Double.IsNaN(endLongitude))
                        {
                            distanceToEnd.Add(L.GetDistanceTo(endLatitude, endLongitude));

                            if (distanceToEnd.getLast() <= HorAccuracy && distanceToEnd.isDecreasing())
                            {
                                loopDetected(currentDistance, __currentTime, L);
                            }
                            /*if (distanceToEnd.getLast() <= HorAccuracy && distanceToEnd.isTourn())
                            {
                                loopDetected();
                            }*/
                        }
                    }
                }

                lastReceived = L;

                return hasToStore;
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("error on position change ", ex);
                return false;
            }
        }

        internal void enableStartAndStop()
        {
            if (ignoreList.ContainsKey(Constants.DO_PAUSE))
            {
                ignoreList.Remove(Constants.DO_PAUSE);
            }
            if (ignoreList.ContainsKey(Constants.DO_CONTINUE))
            {
                ignoreList.Remove(Constants.DO_CONTINUE);
            }
        }
        internal void disableStartAndStop()
        {
            if (!ignoreList.ContainsKey(Constants.DO_PAUSE))
            {
                ignoreList.Add(Constants.DO_PAUSE, Constants.DO_PAUSE);
            }
            if (!ignoreList.ContainsKey(Constants.DO_CONTINUE))
            {
                ignoreList.Add(Constants.DO_CONTINUE, Constants.DO_CONTINUE);
            }
        }


        /*public System.Windows.Media.Color getColor(double res, double margin)
        {
            System.Windows.Media.Color col = System.Windows.Media.Colors.Green;
            int signRes = 0;
            if (Math.Abs(res) >= margin)
                signRes = Math.Sign(res) * 2;
            else
                if (Math.Abs(res) >= (margin / 2))
                signRes = Math.Sign(res);


            if (Math.Abs(signRes) >= 2)
            {
                if (signRes > 0) // slow
                    col = System.Windows.Media.Colors.Red;
                else
                    if (signRes < 0)
                    col = System.Windows.Media.Colors.Blue;
            }
            else
            {
                if (signRes > 0)
                    col = System.Windows.Media.Colors.Orange;
                else
                    if (signRes < 0)
                    col = System.Windows.Media.Colors.Purple;
            }
            return col;
        }*/

    }

}


