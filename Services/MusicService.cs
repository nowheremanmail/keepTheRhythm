using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;

namespace UniversalKeepTheRhythm.Services
{
    /// <summary>
    /// The view model for the player.
    /// </summary>
    /// <remarks>
    /// The view disables the ability to skip during a transition or when
    /// the playback list is empty.
    /// </remarks>
    public class MusicService : INotifyPropertyChanged, IDisposable
    {
        private static readonly object SyncRoot = new object();

        private static MusicService _instance = null;
        public static MusicService instance
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
                        _instance = new MusicService();
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        bool disposed;
        MediaPlayer player;
        //CoreDispatcher dispatcher;
        MediaPlaybackList subscribedPlaybackList;


        public event TypedEventHandler<MediaPlaybackList, CurrentMediaPlaybackItemChangedEventArgs> CurrentItemChanged;
        public event TypedEventHandler<MediaPlaybackList, MediaPlaybackItemOpenedEventArgs> ItemOpened;

        public event TypedEventHandler<MusicService, MediaPlaybackItem> Playing;


        public MediaPlaybackList MediaList
        {
            get { return subscribedPlaybackList; }
            set
            {

                if (subscribedPlaybackList != null)
                {
                    subscribedPlaybackList.CurrentItemChanged -= SubscribedPlaybackList_CurrentItemChanged;
                    subscribedPlaybackList.ItemOpened -= SubscribedPlaybackList_ItemOpened;
                    //subscribedPlaybackList.Items.VectorChanged -= Items_VectorChanged;
                    subscribedPlaybackList = null;
                }

                subscribedPlaybackList = value;

                if (subscribedPlaybackList != null)
                {
                    //if (player.Source != subscribedPlaybackList)
                    player.Source = subscribedPlaybackList;


                    subscribedPlaybackList.ItemOpened += SubscribedPlaybackList_ItemOpened;

                    subscribedPlaybackList.CurrentItemChanged += SubscribedPlaybackList_CurrentItemChanged;
                    //subscribedPlaybackList.Items.VectorChanged += Items_VectorChanged;
                    //HandlePlaybackListChanges(subscribedPlaybackList.Items);
                    player.Play();
                }

                RaisePropertyChanged("MediaList");

            }
        }

        private async void SubscribedPlaybackList_ItemOpened(MediaPlaybackList sender, MediaPlaybackItemOpenedEventArgs args)
        {
            var cur = args.Item;

#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("SubscribedPlaybackList_ItemOpened " + args.Item.ToString());
#endif

            loadProperties(cur);

            ItemOpened?.Invoke(sender, args);
        }

        private async void loadProperties(MediaPlaybackItem cur)
        {
            var mediaItem = cur.Source;
            var file = mediaItem.CustomProperties["FILE"] as StorageFile;

            if (file != null)
            {
                var extraProperties = await file.Properties.GetMusicPropertiesAsync();

                var props = cur.GetDisplayProperties();
                props.MusicProperties.AlbumArtist = extraProperties.AlbumArtist;
                props.MusicProperties.AlbumTitle = extraProperties.Album;
                //props.MusicProperties.AlbumTrackCount = extraProperties.;
                props.MusicProperties.Artist = extraProperties.Artist;
                foreach (var t in extraProperties.Genre) props.MusicProperties.Genres.Add(t);
                props.MusicProperties.Title = extraProperties.Title;
                props.MusicProperties.TrackNumber = extraProperties.TrackNumber;

                cur.ApplyDisplayProperties(props);
                // TODO keep??
                mediaItem.CustomProperties["FILE"] = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MusicService()
        {
            this.player = new MediaPlayer();
            player.AutoPlay = false;
            player.AudioCategory = MediaPlayerAudioCategory.Media;

            //this.dispatcher = dispatcher;

            player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;

        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            if (disposed) return;
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("PlaybackSession_PlaybackStateChanged " + sender.PlaybackState.ToString());
#endif
            /*await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (disposed) return;
                RaisePropertyChanged("PlaybackState");
            });*/

        }

        public void TogglePlayPause()
        {
            switch (player.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    player.Pause();
                    break;
                case MediaPlaybackState.Paused:
                    player.Play();
                    break;
            }
        }

        public void SkipNext()
        {
            var playbackList = player.Source as MediaPlaybackList;
            if (playbackList == null)
                return;

            playbackList.MoveNext();
        }

        public void SkipPrevious()
        {
            var playbackList = player.Source as MediaPlaybackList;
            if (playbackList == null)
                return;

            playbackList.MovePrevious();
        }

        /*private async void Items_VectorChanged(IObservableVector<MediaPlaybackItem> sender, IVectorChangedEventArgs args)
        {
            if (disposed) return;
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (disposed) return;
                HandlePlaybackListChanges(sender);
            });
        }*/


        private void SubscribedPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            if (disposed) return;
#if (DEBUG)
            System.Diagnostics.Debug.WriteLine("SubscribedPlaybackList_CurrentItemChanged " + args.NewItem?.ToString());
#endif

            CurrentItemChanged?.Invoke(sender, args);
            if (args.NewItem != null)
            {
                loadProperties(args.NewItem);
                Playing?.Invoke(this, args.NewItem);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (disposed)
                return;

            if (subscribedPlaybackList != null)
            {
                subscribedPlaybackList.CurrentItemChanged -= SubscribedPlaybackList_CurrentItemChanged;
                subscribedPlaybackList.ItemOpened -= SubscribedPlaybackList_ItemOpened;
                subscribedPlaybackList = null; // Setter triggers vector unsubscribe logic
            }

            player.PlaybackSession.PlaybackStateChanged -= PlaybackSession_PlaybackStateChanged;

            player.Dispose();

            disposed = true;
        }

        internal void Pause()
        {
            if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                player.Pause();
            }
        }

        internal void Play()
        {
            if (player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                player.Play();
            }
        }

        internal async void Start(string name)
        {

            // https://github.com/Microsoft/Windows-universal-samples/tree/master/Samples/BackgroundMediaPlayback
            // http://stackoverflow.com/questions/36383747/uwp-mediaplaybacklist-adding-mediasource-working-too-slow/36474443
            try
            {
                var myMusicLIb = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Music);

                MediaPlaybackList playbackList = new MediaPlaybackList();
                playbackList.AutoRepeatEnabled = true;
                playbackList.ShuffleEnabled = true;

                if (name != "" && name != null)
                {
                    var contentList = await Windows.Media.Playlists.Playlist.LoadAsync(await myMusicLIb.Folders[0].GetFileAsync(name));

                    foreach (var file in contentList.Files)
                    {
                        var mediaSource = Windows.Media.Core.MediaSource.CreateFromStorageFile(file);
                        var mediaItem = new MediaPlaybackItem(mediaSource);

                        mediaSource.CustomProperties["FILE"] = file;

                        //Creating the play list
                        playbackList.Items.Add(mediaItem);
                    }
                }
                else
                {

                    int N = 50;
                    foreach (var f1 in myMusicLIb.Folders)
                    {
#if (DEBUG)
                        System.Diagnostics.Debug.WriteLine("f1 " + f1.Name);
#endif
                        N = await loadSomeFile(playbackList, f1, 50);
                    }

                }

                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MediaList = playbackList;
                });

                /*
                                using (MediaLibrary library = new MediaLibrary())
                                {
                                    if (name == null || "".Equals(name))
                                    {
                                        string currentListName = "";
                                        SongCollection currentList;

                                        currentListName = "";
                                        currentList = library.Songs;

                                        if (overrideCurrent)
                                        {
                                            _currentListName = currentListName;
                                            _currentList = currentList;
                                        }

                                        if (MediaPlayer.State == MediaState.Playing)
                                        {
                                            MediaPlayer.Stop();
                                        }

                                        int N = currentList.Count();
                                        if (N > 0)
                                        {
                                            MediaPlayer.Play(currentList, new Random().Next(N));
                                            MediaPlayer.IsShuffled = true;
                                            MediaPlayer.IsRepeating = true;
                                        }
                                    }
                                    else
                                    {
                                        PlaylistCollection listCol = library.Playlists;
                                        foreach (Playlist p in listCol)
                                        {
                                            if (p.Name.Equals(name))
                                            {
                                                string currentListName = "";
                                                SongCollection currentList;

                                                currentList = p.Songs;
                                                currentListName = p.Name;

                                                if (overrideCurrent)
                                                {
                                                    _currentListName = currentListName;
                                                    _currentList = currentList;
                                                }

                                                if (MediaPlayer.State == MediaState.Playing)
                                                {
                                                    MediaPlayer.Stop();
                                                }

                                                int N = currentList.Count();
                                                if (N > 0)
                                                {
                                                    MediaPlayer.Play(currentList, new Random().Next(N));
                                                    MediaPlayer.IsShuffled = true;
                                                    MediaPlayer.IsRepeating = true;
                                                }
                                            }
                                        }
                                    }
                                }*/
            }
            catch (Exception e)
            {
                nowhereman.LittleWatson.instance.Error("error on action change ", e);
            }
        }

        private async Task<int> loadSomeFile(MediaPlaybackList playbackList, StorageFolder folder, int v)
        {
            foreach (var file in await folder.GetFilesAsync())
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("adding " + file.Name);
#endif
                //var tt = await Windows.Media.Playlists.Playlist.LoadAsync(file);

                //tt.Files

                //await Windows.Media.Playlists.Playlist.LoadAsync(file);



                var mediaSource = Windows.Media.Core.MediaSource.CreateFromStorageFile(file);
                var mediaItem = new MediaPlaybackItem(mediaSource);

                mediaSource.CustomProperties["FILE"] = file;


                //Creating the play list
                playbackList.Items.Add(mediaItem);
                if (playbackList.Items.Count > v) return v;
            }

            if (playbackList.Items.Count > v) return v;
            foreach (var file in await folder.GetFoldersAsync())
            {
#if (DEBUG)
                System.Diagnostics.Debug.WriteLine("checking " + file.Name);
#endif
                v = await loadSomeFile(playbackList, file, v);
                if (playbackList.Items.Count > v) return v;
            }
            return v;
        }
    }
}
