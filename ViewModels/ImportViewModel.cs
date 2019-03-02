using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Mvvm;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Others;
using UniversalKeepTheRhythm.Services;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    public class ImportViewModel : ViewModelBase, IProgress<int>
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();


        double _ProgressValue = 0;
        public double ProgressValue { get { return _ProgressValue; } set { Set(ref _ProgressValue, value); } }

        double _ProgressValueMax = 100;
        public double ProgressValueMax { get { return _ProgressValueMax; } set { Set(ref _ProgressValueMax, value); } }

        bool _progressBar = false;
        public bool progressBar { get { return _progressBar; } set { Set(ref _progressBar, value); } }

        string _ExtraInfo = default(string);
        public string ExtraInfo { get { return _ExtraInfo; } set { Set(ref _ExtraInfo, value); } }


        long _Duration = default(long);
        public long Duration { get { return _Duration; } set { Set(ref _Duration, value); } }

        double _Distance = default(double);
        public double Distance { get { return _Distance; } set { Set(ref _Distance, value); } }


        Collection<Paths> _ListI = default(Collection<Paths>);
        public Collection<Paths> ListI { get { return _ListI; } set { Set(ref _ListI, value); } }

        Paths _SelectedItemI = default(Paths);
        public Paths SelectedItemI { get { return _SelectedItemI; } set { Set(ref _SelectedItemI, value); } }

        string _Comment = default(string);
        public string Comment { get { return _Comment; } set { Set(ref _Comment, value); } }

        string _NamePath = default(string);
        public string NamePath { get { return _NamePath; } set { Set(ref _NamePath, value); } }

        DelegateCommand _ReallyImportCommand;
        public DelegateCommand ReallyImportCommand
           => _ReallyImportCommand ?? (_ReallyImportCommand = new DelegateCommand(async () =>
           {
               await reallyImportAsync();
           }, () => true));

        FileActivatedEventArgs t;
        Paths p;
        Sessions s;

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {
            Comment = "IMPORTED";
            NamePath = "IMPORTED " + string.Format(resourceLoader.GetString("PathName"), DateTime.Now.ToString());

            t = SessionState["importFile"] as FileActivatedEventArgs;

            if (t != null)
            {
                var listI = new ObservableCollection<Paths>();

                p = new Paths();
                p.Id = -1;
                p.Type = "I";
                p.Description = string.Format(resourceLoader.GetString("NewPath"), resourceLoader.GetString("I"));
                listI.Add(p);

                foreach (var t in DataBaseManager.instance.getPaths("I"))
                {
                    listI.Add(t);
                }

                ListI = listI;

                SelectedItemI = p;

                s = new Sessions();
                s.IdPath = p.Id;
                s.DayOfSession = Utils.NOW();
                s.Comment = Comment;

                try
                {
                    progressBar = true;

                    Task.Factory.StartNew(async () =>
                    {
                        string description = null;
                        try
                        {
                            description = await ExportImport.ImportKmlAsync(t.Files[0], true, p, s, this);

                            Duration = (s.Duration.HasValue) ? s.Duration.Value : long.MinValue;
                            Distance = (s.Distance.HasValue) ? s.Distance.Value : double.NaN;
                        }
                        catch (Exception ex)
                        {
                            description = ex.Message;
                        }

                        //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        //{
                        progressBar = false;

                        ExtraInfo = "<html><body>" + description + "</body></html>";
                        //});
                    });
                }
                catch (Exception ex)
                {
                    nowhereman.LittleWatson.instance.Error("import from Kml", ex);

                    progressBar = false;
                    //t0.Text = string.Format("Error {0}", ex.Message);
                }

                SessionState.Remove("importFile");

            }

            return Task.CompletedTask;
        }


        Task reallyImportAsync()
        {
            try
            {
                progressBar = true;

                if (SelectedItemI.Id < 0)
                {
                    p.Description = NamePath;
                    s.Comment = Comment;
                    DataBaseManager.instance.InsertPath(p);
                    s.IdPath = p.Id;
                }
                else
                {
                    s.IdPath = SelectedItemI.Id;
                }

                DataBaseManager.instance.InsertSession(s);

                return Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        if (await ExportImport.ImportKmlAsync(t.Files[0], false, p, s, this) != null)
                        {
                            //NavigationService.GoBack();
                        }
                        progressBar = false;
                    }
                    catch (Exception ex)
                    {
                        progressBar = false;
                        ExtraInfo = ex.Message;
                        nowhereman.LittleWatson.instance.Error("really import from Kml", ex);

                        //t0.Text = string.Format("Error {0}", ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                progressBar = false;
                ExtraInfo = ex.Message;
                nowhereman.LittleWatson.instance.Error("really import from Kml", ex);

                //t0.Text = string.Format("Error {0}", ex.Message);
            }

            return Task.CompletedTask;
        }

        public void Report(int value)
        {
            if (value >= ProgressValueMax)
            {
                ProgressValueMax = value + 1;
            }

            ProgressValue = value;
        }
    }
}
