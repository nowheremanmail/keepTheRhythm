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
    class TotalListViewModel : ViewModelBase
    {
        public readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

        DelegateCommand<Sessions> _RenameCommand;
        public DelegateCommand<Sessions> RenameCommand
           => _RenameCommand ?? (_RenameCommand = new DelegateCommand<Sessions>(async (a) =>
           {
               TextBox inputTextBox = new TextBox();
               inputTextBox.AcceptsReturn = false;
               inputTextBox.Height = 32;
               inputTextBox.Text = a.Comment;
               ContentDialog dialog = new ContentDialog();
               dialog.Content = inputTextBox;
               dialog.Title = resourceLoader.GetString("RenameSession/Text");
               dialog.IsSecondaryButtonEnabled = true;
               dialog.PrimaryButtonText = resourceLoader.GetString("RenameSession/Text");
               dialog.SecondaryButtonText = resourceLoader.GetString("Cancel");
               if (await dialog.ShowAsync() == ContentDialogResult.Primary)
               {
                   DataBaseManager.instance.RenameSession(a, inputTextBox.Text);
                   PathsList = DataBaseManager.instance.GetSessions();
               }
           }, (a) => true));


        DelegateCommand<Sessions> _DeleteCommand;
        public DelegateCommand<Sessions> DeleteCommand
           => _DeleteCommand ?? (_DeleteCommand = new DelegateCommand<Sessions>(async (a) =>
           {
               //MessageBoxResult result = MessageBox.Show(AppResources., AppResources.attention, MessageBoxButton.OKCancel);

               MessageDialog result = new MessageDialog(resourceLoader.GetString("askBeforeRemove"), resourceLoader.GetString("attention"));
               result.Commands.Add(new UICommand(resourceLoader.GetString("DeleteSession/Text"), null, "delete"));
               result.Commands.Add(new UICommand(resourceLoader.GetString("Cancel"), null, ""));
               result.CancelCommandIndex = 0;
               result.DefaultCommandIndex = 0;
               IUICommand selected = await result.ShowAsync();
               if (selected.Id is string && (string)selected.Id == "delete")
               {
                   DataBaseManager.instance.DeleteSession(a, true);
                   PathsList = DataBaseManager.instance.GetSessions();
               }
           }, (a) => true));

        Sessions _SelectedItem = default(Sessions);
        public Sessions SelectedItem { get { return _SelectedItem; } set { Set(ref _SelectedItem, value); } }

    //    public void GotoDetailsPage() =>
    //NavigationService.Navigate(typeof(Views.SessionDetailPage), SelectedItem.Id);
        TappedEventHandler _GotoDetailsPage;
        public TappedEventHandler GotoDetailsPage
           => _GotoDetailsPage ?? (_GotoDetailsPage = new TappedEventHandler((sender, args) =>
           {
               if (SelectedItem != null)
                   NavigationService.Navigate(typeof(Views.SessionDetailPage), SelectedItem.Id);
           }));


        IEnumerable<Sessions> _PathsList = default(List<Sessions>);
        public IEnumerable<Sessions> PathsList { get { return _PathsList; } set { Set(ref _PathsList, value); } }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode modeNav, IDictionary<string, object> state)
        {
            if (modeNav == NavigationMode.New || modeNav == NavigationMode.Refresh || modeNav == NavigationMode.Back)
            {
                PathsList = DataBaseManager.instance.GetSessions();
            }

            return Task.CompletedTask;
        }


    }
}
