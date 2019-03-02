using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Services;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace UniversalKeepTheRhythm.ViewModels
{
    class PathListViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        DelegateCommand<Paths> _RenameCommand;
        public DelegateCommand<Paths> RenameCommand
           => _RenameCommand ?? (_RenameCommand = new DelegateCommand<Paths>(async (a) =>
           {
               TextBox inputTextBox = new TextBox();
               inputTextBox.AcceptsReturn = false;
               inputTextBox.Height = 32;
               inputTextBox.Text = a.Description;
               ContentDialog dialog = new ContentDialog();
               dialog.Content = inputTextBox;
               dialog.Title = resourceLoader.GetString("RenamePath/Text");
               dialog.IsSecondaryButtonEnabled = true;
               dialog.PrimaryButtonText = resourceLoader.GetString("RenamePath/Text");
               dialog.SecondaryButtonText = resourceLoader.GetString("Cancel");
               if (await dialog.ShowAsync() == ContentDialogResult.Primary)
               {
                   DataBaseManager.instance.RenamePath(a, inputTextBox.Text);
                   PathsList = DataBaseManager.instance.getPaths(mode);
               }
           }, (a) => true));


        DelegateCommand<Paths> _DeleteCommand;
        public DelegateCommand<Paths> DeleteCommand
           => _DeleteCommand ?? (_DeleteCommand = new DelegateCommand<Paths>(async (a) =>
           {
               //MessageBoxResult result = MessageBox.Show(AppResources., AppResources.attention, MessageBoxButton.OKCancel);

               MessageDialog result = new MessageDialog(resourceLoader.GetString("askBeforeRemove"), resourceLoader.GetString("attention"));
               result.Commands.Add(new UICommand(resourceLoader.GetString("DeletePath/Text"), null, "delete"));
               result.Commands.Add(new UICommand(resourceLoader.GetString("Cancel"), null, ""));
               result.CancelCommandIndex = 0;
               result.DefaultCommandIndex = 0;
               IUICommand selected = await result.ShowAsync();
               if (selected.Id is string && (string)selected.Id == "delete")
               {
                   DataBaseManager.instance.DeletePath(a);
                   PathsList = DataBaseManager.instance.getPaths(mode);
               }
           }, (a) => true));

        Paths _SelectedItem = default(Paths);
        public Paths SelectedItem { get { return _SelectedItem; } set { Set(ref _SelectedItem, value); } }

    //    public void GotoDetailsPage() =>
    //NavigationService.Navigate(typeof(Views.SessionListPage), SelectedItem.Id);


        TappedEventHandler _GotoDetailsPage;
        public TappedEventHandler GotoDetailsPage
           => _GotoDetailsPage ?? (_GotoDetailsPage = new TappedEventHandler((sender, args) =>
           {
               if (SelectedItem != null)
                   NavigationService.Navigate(typeof(Views.SessionListPage), SelectedItem.Id);
           }));

        string _mode = default(string);
        public string mode { get { return _mode; } set { Set(ref _mode, value); } }

        IEnumerable<Paths> _PathsList = default(List<Paths>);
        public IEnumerable<Paths> PathsList { get { return _PathsList; } set { Set(ref _PathsList, value); } }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {
            if (modeNav == NavigationMode.New || modeNav == NavigationMode.Refresh || modeNav == NavigationMode.Back)
            {
                var paramsTo = parameter as Dictionary<String, object>;

                if (paramsTo.ContainsKey("mode"))
                {

                    mode = paramsTo["mode"] as string;
                }

                PathsList = DataBaseManager.instance.getPaths(mode);
            }

            return Task.CompletedTask;
        }
    }
}
