using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Services;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class NewSessionViewModel : SettingsViewModel
    {
        DelegateCommand _SaveItemCommand;
        public DelegateCommand SaveItemCommand
            => _SaveItemCommand ?? (_SaveItemCommand = new DelegateCommand(() =>
            {
                StartNewAsync();
            }));

        DelegateCommand _SettingsCommand;
        public DelegateCommand SettingsCommand
           => _SettingsCommand ?? (_SettingsCommand = new DelegateCommand(() =>
           {
               NavigationService.Navigate(typeof(Views.SettingsPage), Mode);
           }, () => true));


        int _idPath = -1;
        public int idPath { get { return _idPath; } set { Set(ref _idPath, value); } }
        int _likeid = -1;
        public int likeid { get { return _likeid; } set { Set(ref _likeid, value); } }
        int _idSession = -1;
        public int idSession { get { return _idSession; } set { Set(ref _idSession, value); } }

        //string _mode = "R";
        //public string mode { get { return _mode; } set { Set(ref _mode, value); } }


        Paths _Path = default(Paths);
        public Paths Path { get { return _Path; } set { Set(ref _Path, value); } }

        string _sessionDesc = "";
        public string SessionDesc { get { return _sessionDesc; } set { Set(ref _sessionDesc, value); } }
        //string _pathDesc = "desc";
        //public string pathDesc { get { return _pathDesc; } set { Set(ref _pathDesc, value); } }


        TimeSpan _Duration = default(TimeSpan);
        public TimeSpan Duration
        {
            get { return _Duration; }
            set
            {
                Set(ref _Duration, value);
                RaisePropertyChanged("AvgSpeed");
            }
        }

        double _Distance = default(double);
        public double Distance
        {
            get { return _Distance; }
            set
            {
                Set(ref _Distance, value);
                RaisePropertyChanged("AvgSpeed");
            }
        }

        bool _FixDuration = false;
        public bool FixDuration { get { return _FixDuration; } set { Set(ref _FixDuration, value); } }

        bool _FixDistance = false;
        public bool FixDistance { get { return _FixDistance; } set { Set(ref _FixDistance, value); } }

        Sessions _SelectedItem = default(Sessions);
        public Sessions SelectedItem { get { return _SelectedItem; } set { Set(ref _SelectedItem, value); } }

        IEnumerable<Sessions> _PathsList = default(List<Sessions>);
        public IEnumerable<Sessions> PathsList { get { return _PathsList; } set { Set(ref _PathsList, value); } }

        public double AvgSpeed
        {
            get
            {
                if (Distance > 0.0 && Duration != null)
                    return Duration.TotalMinutes * 1000 / Distance;
                return double.NaN;
            }
        }

        bool _HasGps = true;
        public bool HasGps { get { return _HasGps; } set { Set(ref _HasGps, value); } }

        string _IconStartWorkout = "ms-appx:///Images/appbar.new.rest.png";
        public string IconStartWorkout { get { return _IconStartWorkout; } set { Set(ref _IconStartWorkout, value); } }

        bool _HasAdvices = false;
        public bool HasAdvices { get { return _HasAdvices; } set { Set(ref _HasAdvices, value); } }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            base.OnNavigatingFromAsync(args);

            if (Path != null)
            {
                nowhereman.Properties.setBoolProperty("hasGPS" + Mode, HasGps);
                nowhereman.Properties.setBoolProperty("hasAdvices" + Mode, HasAdvices);

            }
            return Task.CompletedTask;
        }

        //public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        //{
        //    if (suspending)
        //    {
        //        state[nameof(Path)] = Path;
        //        state[nameof(SessionDesc)] = SessionDesc;

        //        state[nameof(Intelligence)] = Intelligence;
        //        state[nameof(IntelligencePace)] = IntelligencePace;
        //        state[nameof(IntelligenceRotation)] = IntelligenceRotation;
        //        state[nameof(HasLoops)] = HasLoops;
        //        state[nameof(LoopDistance)] = LoopDistance;

        //        state[nameof(HasGps)] = HasGps;
        //        state[nameof(HasAdvices)] = HasAdvices;
        //    }
        //    return Task.CompletedTask;
        //}

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {

            var paramsTo = parameter as Dictionary<String, object>;
            Mode = paramsTo["mode"].ToString();

            if (paramsTo.ContainsKey("id"))
                idPath = (int)((Int64)paramsTo["id"]);

            if (paramsTo.ContainsKey("sessionId"))
                likeid = (int)((Int64)paramsTo["sessionId"]);

            if (paramsTo.ContainsKey("idSession"))
                idSession = (int)((Int64)paramsTo["idSession"]);

            base.OnNavigatedToAsync(Mode, modeNav, state);

            if (Mode == "R")
            {
                IconStartWorkout = "ms-appx:///Images/appbar.man.suitcase.run.png";
            }
            else if (Mode == "W")
            {
                IconStartWorkout = "ms-appx:///Images/appbar.man.walk.png";
            }
            else if (Mode == "C")
            {
                IconStartWorkout = "ms-appx:///Images/appbar.bike.png";
            }

            //if (state.Any())
            //{
            //    Path = (Paths)state[nameof(Path)];
            //    SessionDesc = (string)state[nameof(SessionDesc)];

            //    Intelligence = (bool)state[nameof(Intelligence)];
            //    IntelligencePace = (bool)state[nameof(IntelligencePace)];
            //    IntelligenceRotation = (bool)state[nameof(IntelligenceRotation)];
            //    HasLoops = (bool)state[nameof(HasLoops)];
            //    LoopDistance = (double)state[nameof(LoopDistance)];

            //    HasGps = (bool)state[nameof(HasGps)];
            //    HasAdvices = (bool)state[nameof(HasAdvices)];
            //}
            //else
            {
                if (modeNav == NavigationMode.New)
                {
                    if (idPath < 0)
                    {
                        Path = new Paths();
                        Path.Type = Mode;
                        Path.Description = string.Format(resourceLoader.GetString("PathName"), DateTime.Now.ToString());
                        //
                    }
                    else
                    {
                        Path = DataBaseManager.instance.GetPath(idPath);
                    }

                    SessionDesc = "";
                    HasGps = nowhereman.Properties.getBoolProperty("hasGPS" + Mode, true);
                    HasAdvices = nowhereman.Properties.getBoolProperty("hasAdvices" + Mode, true);
                }
            }

            if (modeNav == NavigationMode.New && idPath >= 0)
            {
                getOldData();
            }

            PathsList = DataBaseManager.instance.GetSessions(idPath);


            return Task.CompletedTask;
        }

        private void getOldData()
        {
            double bestPace = Double.MaxValue;

            //long minTimeT = long.MaxValue;
            long maxTimeT = long.MinValue;
            double maxDistanceT = Double.MinValue; //, minDistanceT = Double.MaxValue;

            double startLatitude = Double.NaN, startLongitude = Double.NaN;
            double endLatitude = Double.NaN, endLongitude = Double.NaN;

            bool found = false;

            if (Path != null && idPath >= 0 && likeid < 0)
            {
                foreach (var sess in DataBaseManager.instance.GetSessions(Path.Id))
                {
                    if (sess.Duration != null && sess.Distance != null && sess.AvgPace.Value < bestPace && sess.AvgPace.Value > 0 && sess.Duration.Value > 0 && sess.Distance.Value > 0)
                    {
                        bestPace = sess.AvgPace.Value;

                        //minTimeT = sess.duration.Value < minTimeT ? sess.duration.Value : minTimeT;
                        maxTimeT = sess.Duration.Value; // > maxTimeT ? sess.duration.Value : maxTimeT;


                        //minDistanceT = sess.distance.Value < minDistanceT ? sess.distance.Value : minDistanceT;
                        //if (sess.distance.Value > maxDistanceT)
                        //{
                        maxDistanceT = sess.Distance.Value;

                        if (sess.StartLat != null)
                        {
                            startLatitude = sess.StartLat.Value;
                            startLongitude = sess.StartLon.Value;
                            endLatitude = sess.EndLat.Value;
                            endLongitude = sess.EndLon.Value;
                        }
                        //}

                        found = true;
                    }
                }
            }
            else
            {
                if (Path != null && idPath >= 0 && likeid >= 0)
                {
                    var sess = DataBaseManager.instance.GetSession(likeid);
                    if (sess != null)
                    {
                        if (sess.Duration != null && sess.Distance != null && sess.AvgPace.Value < bestPace && sess.AvgPace.Value > 0 && sess.Duration.Value > 0 && sess.Distance.Value > 0)
                        {
                            bestPace = sess.AvgPace.Value;

                            //minTimeT = sess.duration.Value < minTimeT ? sess.duration.Value : minTimeT;
                            maxTimeT = sess.Duration.Value; // > maxTimeT ? sess.duration.Value : maxTimeT;


                            //minDistanceT = sess.distance.Value < minDistanceT ? sess.distance.Value : minDistanceT;
                            //if (sess.distance.Value > maxDistanceT)
                            //{
                            maxDistanceT = sess.Distance.Value;

                            if (sess.StartLat != null)
                            {
                                startLatitude = sess.StartLat.Value;
                                startLongitude = sess.StartLon.Value;
                                endLatitude = sess.EndLat.Value;
                                endLongitude = sess.EndLon.Value;
                            }
                            //}

                            found = true;
                        }
                    }
                }
            }
            if (found)
            {
                FixDuration = true;
                TimeSpan t = new TimeSpan(maxTimeT);

                double minutes = Math.Floor(t.TotalMinutes);
                t = TimeSpan.FromMinutes(minutes);

                Duration = t;

                if (/*minDistanceT != Double.MaxValue &&*/ maxDistanceT != Double.MinValue)
                {
                    maxDistanceT = Math.Floor(maxDistanceT);

                    FixDistance = true;
                    Distance = maxDistanceT;
                }

                //initialMusicDuration.Value = new TimeSpan(t.Ticks / 10);
                //endMusicDuration.Value = new TimeSpan(t.Ticks * 9 / 10);


                // TODO
                /*
                if (startLatitude != Double.NaN)
                {
                    startPoint.Text = startLatitude.ToString("0.000") + "x" + startLongitude.ToString("0.000");
                    endPoint.Text = endLatitude.ToString("0.000") + "x" + endLongitude.ToString("0.000");

                    // 1 start, end & duration
                    // 2 duration & distance
                    // 3 duration
                    // 4 distance
                    App.engine.setStartPoint(startLatitude, startLongitude);
                    App.engine.setEndPoint(endLatitude, endLongitude);
                }*/
            }

        }

        async void StartNewAsync()
        {
            if (idPath < 0)
            {
                DataBaseManager.instance.InsertPath(Path);
                idPath = Path.Id;
            }
            else
            {
                DataBaseManager.instance.UpdatePath(Path);
            }

            var mode = Path.Type;

            double maxDistanceT = Double.NaN;
            TimeSpan t = TimeSpan.MinValue;
            if (FixDistance)
            {
                maxDistanceT = Distance;
            }
            if (FixDuration)
            {
                t = Duration;
            }

            Engine.instance.setObjective(maxDistanceT, t);

            Engine.instance.setIsLoop(HasLoops, LoopDistance);


            var paramsTo = new Dictionary<String, object>();
            paramsTo.Add("id", idPath);
            paramsTo.Add("text", SessionDesc);
            if (SelectedItem != null)
            {
                paramsTo.Add("followSessionId", SelectedItem.Id);
            }
            else if (likeid > 0)
            {
                paramsTo.Add("followSessionId", likeid);
            }
            paramsTo.Add("mode", mode);

            if (idSession >= 0)
            {
                paramsTo.Add("idSession", idSession);
            }

            await NavigationService.NavigateAsync(typeof(Views.ProgressPage), paramsTo);
        }

    }
}
