using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Template10.Mvvm;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Others;
using UniversalKeepTheRhythm.Services;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();


        double _SummaryHeight = 275;
        public double SummaryHeight { get { return _SummaryHeight; } set { Set(ref _SummaryHeight, value); } }


        double _SummaryWidth = 400;
        public double SummaryWidth { get { return _SummaryWidth; } set { Set(ref _SummaryWidth, value); } }

        DelegateCommand _NewSession;
        public DelegateCommand NewWorkoutCommand
            => _NewSession ?? (_NewSession = new DelegateCommand(() =>
            {
                StartNewSession();
            }));


        DelegateCommand _SettingsCommand;
        public DelegateCommand SettingsCommand
           => _SettingsCommand ?? (_SettingsCommand = new DelegateCommand(() =>
           {
               PivotItem tmp = SelectedPivotItem;
               if (tmp != null)
               {
                   string t = tmp.Tag as string;
                   if (t == "T" || t == null)
                       NavigationService.Navigate(typeof(Views.GeneralSettingsPage));
                   else
                       NavigationService.Navigate(typeof(Views.SettingsPage), t);
               }
               else
               {
                   NavigationService.Navigate(typeof(Views.GeneralSettingsPage));
               }
           }, () => true));


        DelegateCommand _HistoryCommand;
        public DelegateCommand HistoryCommand
           => _HistoryCommand ?? (_HistoryCommand = new DelegateCommand(() =>
           {
               seePathList();
           }, () => true));

        DelegateCommand _ImportedCommand;
        public DelegateCommand ImportedCommand
           => _ImportedCommand ?? (_ImportedCommand = new DelegateCommand(() =>
           {
               SeeImportedList();
           }, () => true));


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


        TappedEventHandler _SeeLastSession;
        public TappedEventHandler SeeLastSession
           => _SeeLastSession ?? (_SeeLastSession = new TappedEventHandler(async (sender, args) =>
           {
               if (lastSession != null)
                   await NavigationService.NavigateAsync(typeof(Views.SessionDetailPage), lastSession.Id);
           }));


        TappedEventHandler _LongestDistance_Tap;
        public TappedEventHandler LongestDistance_Tap
           => _LongestDistance_Tap ?? (_LongestDistance_Tap = new TappedEventHandler(async (sender, args) =>
           {
               PivotItem tmp = SelectedPivotItem;
               if (tmp != null)
               {
                   string t = tmp.Tag as string;
                   int id = DataBaseManager.instance.getLongestDistance(t);
                   if (id > 0)
                   {
                       await NavigationService.NavigateAsync(typeof(Views.SessionDetailPage), id);
                   }
               }
           }));


        TappedEventHandler _LongestTime_Tap;
        public TappedEventHandler LongestTime_Tap
           => _LongestTime_Tap ?? (_LongestTime_Tap = new TappedEventHandler(async (sender, args) =>
           {
               PivotItem tmp = SelectedPivotItem;
               if (tmp != null)
               {
                   string t = tmp.Tag as string;
                   int id = DataBaseManager.instance.getLongestTime(t);
                   if (id > 0)
                   {
                       await NavigationService.NavigateAsync(typeof(Views.SessionDetailPage), id);
                   }
               }
           }));

        TappedEventHandler _HigherSpeed_Tap;
        public TappedEventHandler HigherSpeed_Tap
           => _HigherSpeed_Tap ?? (_HigherSpeed_Tap = new TappedEventHandler(async (sender, args) =>
           {
               PivotItem tmp = SelectedPivotItem;
               if (tmp != null)
               {
                   string t = tmp.Tag as string;
                   int id = DataBaseManager.instance.getHigherSpeed(t);
                   if (id > 0)
                   {
                       await NavigationService.NavigateAsync(typeof(Views.SessionDetailPage), id);
                   }
               }
           }));

        TappedEventHandler _HigherPace_Tap;
        public TappedEventHandler HigherPace_Tap
           => _HigherPace_Tap ?? (_HigherPace_Tap = new TappedEventHandler(async (sender, args) =>
           {
               PivotItem tmp = SelectedPivotItem;
               if (tmp != null)
               {
                   string t = tmp.Tag as string;
                   int id = DataBaseManager.instance.getHigherPace(t);
                   if (id > 0)
                   {
                       await NavigationService.NavigateAsync(typeof(Views.SessionDetailPage), id);
                   }
               }
           }));

        bool _AcceptConditionsPopup = true;
        public bool AcceptConditionsPopup { get { return _AcceptConditionsPopup; } set { Set(ref _AcceptConditionsPopup, value); } }

        DelegateCommand _AcceptConditionsCommand;
        public DelegateCommand AcceptConditionsCommand
           => _AcceptConditionsCommand ?? (_AcceptConditionsCommand = new DelegateCommand(() =>
           {
               AcceptConditionsPopup = false;
           }, () => true));

        int _AbsoluteWorkoutsT = default(int);
        public int AbsoluteWorkoutsT { get { return _AbsoluteWorkoutsT; } set { Set(ref _AbsoluteWorkoutsT, value); } }

        double _AbsoluteDistanceT = default(double);
        public double AbsoluteDistanceT { get { return _AbsoluteDistanceT; } set { Set(ref _AbsoluteDistanceT, value); } }

        string _LastSessionT = default(string);
        public string LastSessionT { get { return _LastSessionT; } set { Set(ref _LastSessionT, value); } }

        TimeSpan _AbsoluteTimeT = default(TimeSpan);
        public TimeSpan AbsoluteTimeT { get { return _AbsoluteTimeT; } set { Set(ref _AbsoluteTimeT, value); } }

        Collection<Paths> _ListT = default(Collection<Paths>);
        public Collection<Paths> ListT { get { return _ListT; } set { Set(ref _ListT, value); } }

        bool _CountI = default(bool);
        public bool CountI { get { return _CountI; } set { Set(ref _CountI, value); } }

        double _LongestDistanceT = default(double);
        public double LongestDistanceT { get { return _LongestDistanceT; } set { Set(ref _LongestDistanceT, value); } }

        TimeSpan _LongestTimeT = default(TimeSpan);
        public TimeSpan LongestTimeT { get { return _LongestTimeT; } set { Set(ref _LongestTimeT, value); } }

        double _HigherSpeedT = default(double);
        public double HigherSpeedT { get { return _HigherSpeedT; } set { Set(ref _HigherSpeedT, value); } }

        double _HigherPaceT = default(double);
        public double HigherPaceT { get { return _HigherPaceT; } set { Set(ref _HigherPaceT, value); } }

        Visibility _SummaryVisibility = Visibility.Visible;
        public Visibility SummaryVisibility { get { return _SummaryVisibility; } set { Set(ref _SummaryVisibility, value); } }

        int _AbsoluteWorkoutsR = default(int);
        public int AbsoluteWorkoutsR { get { return _AbsoluteWorkoutsR; } set { Set(ref _AbsoluteWorkoutsR, value); } }

        double _AbsoluteDistanceR = default(double);
        public double AbsoluteDistanceR { get { return _AbsoluteDistanceR; } set { Set(ref _AbsoluteDistanceR, value); } }

        double _LastSessionR = default(double);
        public double LastSessionR { get { return _LastSessionR; } set { Set(ref _LastSessionR, value); } }

        TimeSpan _AbsoluteTimeR = default(TimeSpan);
        public TimeSpan AbsoluteTimeR { get { return _AbsoluteTimeR; } set { Set(ref _AbsoluteTimeR, value); } }

        Collection<Paths> _ListR = default(Collection<Paths>);
        public Collection<Paths> ListR { get { return _ListR; } set { Set(ref _ListR, value); } }

        bool _CountR = default(bool);
        public bool CountR { get { return _CountR; } set { Set(ref _CountR, value); } }
        double _LongestDistanceR = default(double);
        public double LongestDistanceR { get { return _LongestDistanceR; } set { Set(ref _LongestDistanceR, value); } }

        TimeSpan _LongestTimeR = default(TimeSpan);
        public TimeSpan LongestTimeR { get { return _LongestTimeR; } set { Set(ref _LongestTimeR, value); } }

        double _HigherSpeedR = default(double);
        public double HigherSpeedR { get { return _HigherSpeedR; } set { Set(ref _HigherSpeedR, value); } }

        double _HigherPaceR = default(double);
        public double HigherPaceR { get { return _HigherPaceR; } set { Set(ref _HigherPaceR, value); } }



        int _AbsoluteWorkoutsW = default(int);
        public int AbsoluteWorkoutsW { get { return _AbsoluteWorkoutsW; } set { Set(ref _AbsoluteWorkoutsW, value); } }

        double _AbsoluteDistanceW = default(double);
        public double AbsoluteDistanceW { get { return _AbsoluteDistanceW; } set { Set(ref _AbsoluteDistanceW, value); } }

        double _LastSessionW = default(double);
        public double LastSessionW { get { return _LastSessionW; } set { Set(ref _LastSessionW, value); } }

        TimeSpan _AbsoluteTimeW = default(TimeSpan);
        public TimeSpan AbsoluteTimeW { get { return _AbsoluteTimeW; } set { Set(ref _AbsoluteTimeW, value); } }

        Collection<Paths> _ListW = default(Collection<Paths>);
        public Collection<Paths> ListW { get { return _ListW; } set { Set(ref _ListW, value); } }

        bool _CountW = default(bool);
        public bool CountW { get { return _CountW; } set { Set(ref _CountW, value); } }
        double _LongestDistanceW = default(double);
        public double LongestDistanceW { get { return _LongestDistanceW; } set { Set(ref _LongestDistanceW, value); } }

        TimeSpan _LongestTimeW = default(TimeSpan);
        public TimeSpan LongestTimeW { get { return _LongestTimeW; } set { Set(ref _LongestTimeW, value); } }

        double _HigherSpeedW = default(double);
        public double HigherSpeedW { get { return _HigherSpeedW; } set { Set(ref _HigherSpeedW, value); } }

        double _HigherPaceW = default(double);
        public double HigherPaceW { get { return _HigherPaceW; } set { Set(ref _HigherPaceW, value); } }


        int _AbsoluteWorkoutsC = default(int);
        public int AbsoluteWorkoutsC { get { return _AbsoluteWorkoutsC; } set { Set(ref _AbsoluteWorkoutsC, value); } }

        double _AbsoluteDistanceC = default(double);
        public double AbsoluteDistanceC { get { return _AbsoluteDistanceC; } set { Set(ref _AbsoluteDistanceC, value); } }

        double _LastSessionC = default(double);
        public double LastSessionC { get { return _LastSessionC; } set { Set(ref _LastSessionC, value); } }

        TimeSpan _AbsoluteTimeC = default(TimeSpan);
        public TimeSpan AbsoluteTimeC { get { return _AbsoluteTimeC; } set { Set(ref _AbsoluteTimeC, value); } }

        Collection<Paths> _ListC = default(Collection<Paths>);
        public Collection<Paths> ListC { get { return _ListC; } set { Set(ref _ListC, value); } }

        bool _CountC = default(bool);
        public bool CountC { get { return _CountC; } set { Set(ref _CountC, value); } }
        double _LongestDistanceC = default(double);
        public double LongestDistanceC { get { return _LongestDistanceC; } set { Set(ref _LongestDistanceC, value); } }

        TimeSpan _LongestTimeC = default(TimeSpan);
        public TimeSpan LongestTimeC { get { return _LongestTimeC; } set { Set(ref _LongestTimeC, value); } }

        double _HigherSpeedC = default(double);
        public double HigherSpeedC { get { return _HigherSpeedC; } set { Set(ref _HigherSpeedC, value); } }

        double _HigherPaceC = default(double);
        public double HigherPaceC { get { return _HigherPaceC; } set { Set(ref _HigherPaceC, value); } }

        string _TextTime = default(string);
        public string TextTime { get { return _TextTime; } set { Set(ref _TextTime, value); } }

        string _TextDistance = default(string);
        public string TextDistance { get { return _TextDistance; } set { Set(ref _TextDistance, value); } }

        PointCollection _Times = default(PointCollection);
        public PointCollection Times { get { return _Times; } set { Set(ref _Times, value); } }

        PointCollection _Distances = default(PointCollection);
        public PointCollection Distances { get { return _Distances; } set { Set(ref _Distances, value); } }

        PointCollection _Sub = default(PointCollection);
        public PointCollection SubLines { get { return _Sub; } set { Set(ref _Sub, value); } }

        double _TextCanvasTop = 164;
        public double TextCanvasTop { get { return _TextCanvasTop; } set { Set(ref _TextCanvasTop, value); } }

        private Sessions lastSession = null;
        private Paths lastPath = null;

        Paths _SelectedItemR = default(Paths);
        public Paths SelectedItemR { get { return _SelectedItemR; } set { Set(ref _SelectedItemR, value); } }

        Paths _SelectedItemW = default(Paths);
        public Paths SelectedItemW { get { return _SelectedItemW; } set { Set(ref _SelectedItemW, value); } }

        Paths _SelectedItemC = default(Paths);
        public Paths SelectedItemC { get { return _SelectedItemC; } set { Set(ref _SelectedItemC, value); } }

        PivotItem _SelectedPivotItem = default(PivotItem);
        public PivotItem SelectedPivotItem { get { return _SelectedPivotItem; } set { Set(ref _SelectedPivotItem, value); } }

        IDictionary<string, List<object>> dict = null;

        SizeChangedEventHandler _SizeChanged;
        public SizeChangedEventHandler SizeChanged
           => _SizeChanged ?? (_SizeChanged = new SizeChangedEventHandler((sender, args) =>
           {
               SummaryHeight = args.NewSize.Height;
               SummaryWidth = args.NewSize.Width;
               calculateCanvas();
           }));

        private async void seePathList()
        {
            var paramsTo = new Dictionary<String, object>();
            try
            {
                PivotItem tmp = SelectedPivotItem;
                if (tmp != null)
                {
                    string t = tmp.Tag as string;

                    if ("R".Equals(t))
                    {
                        paramsTo.Add("mode", t);
                    }
                    else if ("W".Equals(t))
                    {
                        paramsTo.Add("mode", t);
                    }
                    else if ("C".Equals(t))
                    {
                        paramsTo.Add("mode", t);
                    }
                    else if ("T".Equals(t))
                    {
                        await NavigationService.NavigateAsync(typeof(Views.TotalListPage), paramsTo);
                        return;
                    }

                    await NavigationService.NavigateAsync(typeof(Views.PathListPage), paramsTo);
                }
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("ApplicationBarIconButton_Click", ex);
            }

        }

        private async void SeeImportedList()
        {
            var paramsTo = new Dictionary<String, object>();
                paramsTo.Add("mode", "I");
                await NavigationService.NavigateAsync(typeof(Views.PathListPage), paramsTo);
        }

        private async void StartNewSession()
        {
            var paramsTo = new Dictionary<String, object>();

            try
            {
                PivotItem tmp = SelectedPivotItem;
                if (tmp != null)
                {
                    string t = tmp.Tag as string;

                    if ("R".Equals(t))
                    {
                        var path = SelectedItemR;
                        paramsTo.Add("mode", t);
                        if (path != null) paramsTo.Add("id", path.Id);
                    }
                    else if ("W".Equals(t))
                    {
                        var path = SelectedItemW;
                        paramsTo.Add("mode", t);
                        if (path != null) paramsTo.Add("id", path.Id);
                    }
                    else if ("C".Equals(t))
                    {
                        var path = SelectedItemC;
                        paramsTo.Add("mode", t);
                        if (path != null) paramsTo.Add("id", path.Id);
                    }
                    else if ("T".Equals(t))
                    {
                        if (lastSession != null)
                        {
                            paramsTo.Add("sessionId", lastSession.Id);
                            paramsTo.Add("id", lastSession.IdPath);
                            paramsTo.Add("mode", lastPath.Type);
                        }
                        else
                        {
                            paramsTo.Add("mode", "R");
                        }
                    }

                    await NavigationService.NavigateAsync(typeof(Views.NewSessionPage), paramsTo);
                }
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("ApplicationBarIconButton_Click", ex);
            }
        }


        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (!AcceptConditions)
            {
                if (!nowhereman.Properties.exists("hasLoops" + "R"))
                {
                    nowhereman.Properties.setBoolProperty("hasLoops" + "R", true);
                }

                if (!nowhereman.Properties.exists("modePace" + "C"))
                {
                    nowhereman.Properties.setBoolProperty("modePace" + "C", false);
                }

                if (!nowhereman.Properties.exists("followCompass" + "C"))
                {
                    nowhereman.Properties.setBoolProperty("followCompass" + "C", true);
                }

                if (!nowhereman.Properties.exists("intelligence" + "W"))
                {
                    nowhereman.Properties.setBoolProperty("intelligence" + "W", false);
                }

                if (!nowhereman.Properties.exists("intelligenceRotation" + "R"))
                {
                    nowhereman.Properties.setBoolProperty("intelligenceRotation" + "R", true);
                }

                AcceptConditionsPopup = true;
            }
            else
            {
                AcceptConditionsPopup = false;

                if (mode == NavigationMode.New)
                {
                    try
                    {
                        nowhereman.LittleWatson.instance.CheckForPreviousExceptionAsync(resourceLoader.GetString("emailNWM"), "Error Report [" + resourceLoader.GetString("app_name") + "]",
                            resourceLoader.GetString("Watson0"), resourceLoader.GetString("Watson1"));
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            var listR = new ObservableCollection<Paths>();
            var listC = new ObservableCollection<Paths>();
            var listW = new ObservableCollection<Paths>();
            //listI = new ObservableCollection<Paths>();

            Paths tmpR = new Paths();
            tmpR.Id = -1;
            tmpR.Description = string.Format(resourceLoader.GetString("NewPath"), resourceLoader.GetString("R"));
            tmpR.Type = "R";
            listR.Add(tmpR);

            Paths tmpC = new Paths();
            tmpC.Id = -1;
            tmpC.Description = string.Format(resourceLoader.GetString("NewPath"), resourceLoader.GetString("C"));
            tmpC.Type = "C";
            listC.Add(tmpC);

            Paths tmpW = new Paths();
            tmpW.Id = -1;
            tmpW.Description = string.Format(resourceLoader.GetString("NewPath"), resourceLoader.GetString("W"));
            tmpW.Type = "W";
            listW.Add(tmpW);

            CountR = false;
            CountW = false;
            CountC = false;
            CountI = false;

            foreach (var t in DataBaseManager.instance.getPaths(null))
            {
                if ("R".Equals(t.Type))
                {
                    listR.Add(t);
                    CountR = true;
                }
                if ("C".Equals(t.Type))
                {
                    listC.Add(t);
                    CountC = true;
                }
                if ("W".Equals(t.Type))
                {
                    listW.Add(t);
                    CountW = true;
                }
                if ("I".Equals(t.Type))
                {
                    //listI.Add(t);
                    CountI = true;
                }
            }

            ListC = listC;
            ListR = listR;
            ListW = listW;

            SelectedItemC = tmpC;
            SelectedItemR = tmpR;
            SelectedItemW = tmpW;

            lastSession = DataBaseManager.instance.getLastSession();
            if (lastSession != null)
            {
                lastPath = DataBaseManager.instance.GetPath(lastSession.IdPath);
            }
            var d = DataBaseManager.instance.getTotals();

            if (nowhereman.Properties.getBoolProperty("showGraph", true))
            {
                dict = DataBaseManager.instance.getSummary();
            }

            calculateCanvas();

            if (lastSession != null&& lastPath!= null)
            {
                String unit = nowhereman.Properties.getProperty("units", "m");
                LastSessionT = resourceLoader.GetString(lastPath.Type) + "\n";
                if (lastSession.Distance.HasValue && lastSession.Duration.HasValue)
                {
                    LastSessionT += Utils.toDistanceString(lastSession.Distance.Value, unit, CultureInfo.CurrentCulture) + "\n" + Utils.toTimeString(lastSession.Duration.Value);
                }
                else
                {
                    if (lastSession.Distance.HasValue)
                    {
                        LastSessionT += Utils.toDistanceString(lastSession.Distance.Value, unit, CultureInfo.CurrentCulture);
                    }
                    else
                        if (lastSession.Duration.HasValue)
                    {
                        LastSessionT += Utils.toTimeString(lastSession.Duration.Value);
                    }

                }
            }
            else
            {
                LastSessionT = "";
            }

            //IDictionary<string, string> d = ee.UserState as IDictionary<string, string>;

            string le = "T";
            if (d.ContainsKey("totalNumber" + le))
            {
                AbsoluteWorkoutsT = (int)d["totalNumber" + le];
                AbsoluteDistanceT = (double)d["totalDistance" + le];
                AbsoluteTimeT = TimeSpan.FromTicks((long)d["totalTime" + le]);
                /*longestDistanceT = d["maxDistance" + le];
                longestTimeT = d["maxDuration" + le];
                higherSpeedT = d["maxSpeed" + le];
                higherPaceT = d["maxPace" + le];*/
            }
            else
            {
                AbsoluteWorkoutsT = 0;
                AbsoluteDistanceT = 0.0;
                AbsoluteTimeT = TimeSpan.MinValue;
            }
            le = "R";
            if (CountR && d.ContainsKey("totalNumber" + le))
            {
                AbsoluteWorkoutsR = (int)d["totalNumber" + le];
                AbsoluteDistanceR = (double)d["totalDistance" + le];
                AbsoluteTimeR = TimeSpan.FromTicks((long)d["totalTime" + le]);
                LongestDistanceR = (double)d["maxDistance" + le];
                LongestTimeR = TimeSpan.FromTicks((long)d["maxDuration" + le]);
                HigherSpeedR = (double)d["maxSpeed" + le];
                HigherPaceR = (double)d["maxPace" + le];
            }
            else
            {
                AbsoluteWorkoutsR = 0;
                AbsoluteDistanceR = 0.0;
                AbsoluteTimeR = TimeSpan.MinValue;
                LongestDistanceR = 0;
                LongestTimeR = TimeSpan.MinValue;
                HigherSpeedR = 0.0;
                HigherPaceR = 0.0;
            }

            le = "W";
            if (CountW && d.ContainsKey("totalNumber" + le))
            {
                AbsoluteWorkoutsW = (int)d["totalNumber" + le];
                AbsoluteDistanceW = (double)d["totalDistance" + le];
                AbsoluteTimeW = TimeSpan.FromTicks((long)d["totalTime" + le]);
                LongestDistanceW = (double)d["maxDistance" + le];
                LongestTimeW = TimeSpan.FromTicks((long)d["maxDuration" + le]);
                HigherSpeedW = (double)d["maxSpeed" + le];
                HigherPaceW = (double)d["maxPace" + le];
            }
            else
            {
                AbsoluteWorkoutsW = 0;
                AbsoluteDistanceW = 0.0;
                AbsoluteTimeW = TimeSpan.MinValue;
                LongestDistanceW = 0;
                LongestTimeW = TimeSpan.MinValue;
                HigherSpeedW = 0.0;
                HigherPaceW = 0.0;
            }


            le = "C";
            if (CountC && d.ContainsKey("totalNumber" + le))
            {
                AbsoluteWorkoutsC = (int)d["totalNumber" + le];
                AbsoluteDistanceC = (double)d["totalDistance" + le];
                AbsoluteTimeC = TimeSpan.FromTicks((long)d["totalTime" + le]);
                LongestDistanceC = (double)d["maxDistance" + le];
                LongestTimeC = TimeSpan.FromTicks((long)d["maxDuration" + le]);
                HigherSpeedC = (double)d["maxSpeed" + le];
                HigherPaceC = (double)d["maxPace" + le];
            }
            else
            {
                AbsoluteWorkoutsC = 0;
                AbsoluteDistanceC = 0.0;
                AbsoluteTimeC = TimeSpan.MinValue;
                LongestDistanceC = 0;
                LongestTimeC = TimeSpan.MinValue;
                HigherSpeedC = 0.0;
                HigherPaceC = 0.0;
            }



            //PivotItem tmp = pivotCtl.SelectedItem as PivotItem;
            //    if (tmp != null)
            //    {
            //        string t = tmp.Tag as string;

            //        if ("T".Equals(t))
            //        {
            //            button1.IsEnabled = lastSession != null;
            //            button2.IsEnabled = true;
            //            button3.IsEnabled = false;
            //        }
            //        else
            //        {
            //            button1.IsEnabled = true;
            //            button2.IsEnabled = true;
            //            button3.IsEnabled = false;

            //            if ("R".Equals(t))
            //            {
            //                button3.IsEnabled = countR;
            //            }
            //            else
            //                if ("W".Equals(t))
            //            {
            //                button3.IsEnabled = countW;
            //            }
            //            else
            //                    if ("C".Equals(t))
            //            {
            //                button3.IsEnabled = countC;
            //            }
            //        }
            //    }

            return Task.CompletedTask;
        }

        private void calculateCanvas()
        {
            if (dict != null && nowhereman.Properties.getBoolProperty("showGraph", true))
            {
                SummaryVisibility = Visibility.Visible;

                List<object> days = dict["days"];

                int N = days.Count;

                string gi = nowhereman.Properties.getProperty("graphInfo", "td");
                bool showt = gi.IndexOf('t') >= 0;
                bool showd = gi.IndexOf('d') >= 0;

                List<object> time = dict["time"];
                List<object> distance = dict["distance"];
                List<object> subs = dict["sub"];

                double distanceMax = 0;
                long timeMax = 0;

                foreach (var t in time)
                {
                    long tt = (long)t;
                    if (tt > timeMax)
                    {
                        timeMax = tt;
                    }
                }

                foreach (var d in distance)
                {
                    double dd = (double)d;
                    if (dd > distanceMax)
                    {
                        distanceMax = dd;
                    }
                }

                double WIDTH = SummaryWidth;
                double _HEIGH = SummaryHeight;
                double HEIGH = SummaryHeight;

                double dW = WIDTH / N;
                double dHt = timeMax > 0 ? HEIGH / (double)timeMax : 1.0;
                double dHd = distanceMax > 0 ? HEIGH / distanceMax : 1.0;

                var times = new PointCollection();
                var distances = new PointCollection();
                var sub = new PointCollection();

                double currentX = 0;
                for (int i = 0; i < N; i++)
                {
                    double u = dHt * (double)((long)time[i]);
                    double v = dHd * (double)distance[i];

                    if (showt && showd)
                    {
                        times.Add(new Point(currentX, _HEIGH - 0));
                        times.Add(new Point(currentX, _HEIGH - u));
                        times.Add(new Point(currentX + dW / 2 - 1, _HEIGH - u));
                        times.Add(new Point(currentX + dW / 2 - 1, _HEIGH - 0));

                        distances.Add(new Point(currentX + dW / 2, _HEIGH - 0));
                        distances.Add(new Point(currentX + dW / 2, _HEIGH - v));
                        distances.Add(new Point(currentX + dW - 1, _HEIGH - v));
                        distances.Add(new Point(currentX + dW - 1, _HEIGH - 0));
                    }
                    else if (showd)
                    {
                        distances.Add(new Point(currentX, _HEIGH - 0));
                        distances.Add(new Point(currentX, _HEIGH - v));
                        distances.Add(new Point(currentX + dW - 1, _HEIGH - v));
                        distances.Add(new Point(currentX + dW - 1, _HEIGH - 0));
                    }
                    else if (showt)
                    {
                        times.Add(new Point(currentX, _HEIGH - 0));
                        times.Add(new Point(currentX, _HEIGH - u));
                        times.Add(new Point(currentX + dW - 1, _HEIGH - u));
                        times.Add(new Point(currentX + dW - 1, _HEIGH - 0));
                    }

                    if ((bool)subs[i])
                    {
                        sub.Add(new Point(currentX, _HEIGH + 1));
                        sub.Add(new Point(currentX, _HEIGH + 3));
                        sub.Add(new Point(currentX, _HEIGH + 1));
                    }
                    else
                    {
                        if (sub.Count <= 0)
                        {
                            sub.Add(new Point(currentX, _HEIGH + 1));
                        }
                    }
                    currentX += dW + 1;
                }

                sub.Add(new Point(currentX, _HEIGH + 1));
                if (showt)
                {
                    times.Add(new Point(0, _HEIGH - 0));
                }

                if (showd)
                {
                    distances.Add(new Point(0, _HEIGH - 0));
                }
                Times = times;
                Distances = distances;
                SubLines = sub;
                //sparkLine.Points = sub;

                TextCanvasTop = _HEIGH + 2;
                //t2.SetValue(Canvas.TopProperty, _HEIGH + 2);

                String unit = nowhereman.Properties.getProperty("units", "m");
                if (showd)
                    TextDistance = string.Format(resourceLoader.GetString("maxValueD"), Utils.toDistanceString(distanceMax, unit, CultureInfo.CurrentCulture));
                else
                    TextDistance = "";

                if (showt)
                    TextTime = string.Format(resourceLoader.GetString("maxValueT"), Utils.toTimeString((long)timeMax));
                else
                    TextTime = "";

            }
            else
            {
                SummaryVisibility = Visibility.Collapsed;
            }
        }

    }
}
