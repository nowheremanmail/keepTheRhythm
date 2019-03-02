using Microsoft.Toolkit.Services.OneDrive;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace UniversalKeepTheRhythm.Services
{
    public class LiveApi
    {
        private static string APP_ID = "xxx";
        private static string redirectUri = null; //"urn:ietf:wg:oauth:2.0:oob";
        private string skyDriveFolderName = "keepTheRhythm";
        OneDriveStorageFolder skyDriveFolder;
        private string[] scopes = new[] { Microsoft.Toolkit.Services.Services.MicrosoftGraph.MicrosoftGraphScope.FilesReadWriteAll };

        public static readonly string DOWNLOADING = "downloading";
        public static readonly string DOWNLOAD_FAILED = "downloadfailed";
        public static readonly string FILE_NOT_FOUND = "filenotfound";
        public static readonly string FOLDER_NOT_FOUND = "foldernotfound";
        public static readonly string TASK_CANCELLED = "taskcancelled";
        public static readonly string ERROR = "error";
        public static readonly string CONNECTED = "connected";
        public static readonly string FOLDER_CREATED = "foldercreated";
        public static readonly string FOLDER_FOUND = "folderfound";
        public static readonly string NOT_CONNECTED = "notconnected";
        public static readonly string CONNECTING = "connecting";
        public static readonly string SHARING = "sharing";
        public static readonly string UPLOADING = "uploading";

        public delegate void DownloadOperationEv(LiveApi sender, IStorageFile file);
        public event DownloadOperationEv downloadFinished;

        public delegate void UploadOperationEv(LiveApi sender, IStorageFile file);
        public event UploadOperationEv uploadFinished;

        public delegate void LiveStatusChange(LiveApi sender, double status);
        public event LiveStatusChange notifChange;

        public delegate void LiveStatusMessage(LiveApi sender, string msg, Exception ex);
        public event LiveStatusMessage notifMessage;

        public System.Threading.CancellationTokenSource ctsUpload = null;
        public System.Threading.CancellationTokenSource ctsDownload = null;
        //private IProgress<LiveOperationProgress> progressHandler;

        public LiveApi(string folder, string[] sco)
        {
            //APP_ID = app;
            skyDriveFolderName = folder;
            if (sco != null)
            {
                scopes = sco;
            }

            ctsUpload = new System.Threading.CancellationTokenSource();
            ctsDownload = new System.Threading.CancellationTokenSource();

            Microsoft.Toolkit.Services.OneDrive.OneDriveService.Instance.Initialize(APP_ID, scopes, null, redirectUri);
        }


        public void reset()
        {
            ctsUpload = new System.Threading.CancellationTokenSource();
            ctsDownload = new System.Threading.CancellationTokenSource();
        }

        /*
                private void DownloadProgress(DownloadOperation download)
                {

                    System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Progress: {0}, Status: {1}", download.Guid, download.Progress.Status));

                    double percent = 100;
                    if (download.Progress.TotalBytesToReceive > 0)
                    {
                        percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive;
                    }

                    System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, " - Transfered bytes: {0} of {1}, {2}%", download.Progress.BytesReceived, download.Progress.TotalBytesToReceive, percent));

                    if (download.Progress.HasRestarted)
                    {
                        System.Diagnostics.Debug.WriteLine(" - Download restarted");
                    }

                    if (download.Progress.HasResponseChanged)
                    {
                        // We've received new response headers from the server.
                        System.Diagnostics.Debug.WriteLine(" - Response updated; Header count: " + download.GetResponseInformation().Headers.Count);

                        // If you want to stream the response data this is a good time to start.
                        // download.GetResultStreamAt(0);
                    }
                }
        */


        // login windows http://msdn.microsoft.com/en-us/library/hh968445.aspx


        private async Task<string> obtainSessionAsync()
        {
            if (await OneDriveService.Instance.LoginAsync())
            {

                LiveStatusMessage notif = notifMessage;
                if (notif != null)
                {
                    notif(this, CONNECTED, null);
                }


                return "OK";
            }
            else
            {

                LiveStatusMessage notif = notifMessage;
                if (notif != null)
                {
                    notif(this, NOT_CONNECTED, null);
                }

            }
            return "KO";
        }


        private async Task<string> connectAsync()
        {
            string res = await obtainSessionAsync();

            if (res == "OK")
            {
                var rootFolder = await OneDriveService.Instance.RootFolderForMeAsync();

                skyDriveFolder = await rootFolder.StorageFolderPlatformService.CreateFolderAsync(skyDriveFolderName, CreationCollisionOption.OpenIfExists);
                return "OK";
            }


            return "KO";

        }


        public async Task<string> downloadFileAsync(string filename, IStorageFile file)
        {
            try
            {
                string tmp = await connectAsync();

                if (tmp == "OK")
                {
                    var sourceItem = await skyDriveFolder.GetFileAsync(filename);
                    if (sourceItem != null)
                    {
                        using (var remoteStream = (await sourceItem.StorageFilePlatformService.OpenAsync()) as IRandomAccessStream)
                        {
                            byte[] buffer = new byte[remoteStream.Size];
                            var localBuffer = await remoteStream.ReadAsync(buffer.AsBuffer(), (uint)remoteStream.Size, InputStreamOptions.ReadAhead);

                            using (var localStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                //var res = await RandomAccessStream.CopyAsync(remoteStream, localStream);
                                await localStream.WriteAsync(localBuffer);
                                if (await localStream.FlushAsync())
                                {

                                }
                                //if (res == 1)
                                {

                                }
                            }
                        }

                        if (downloadFinished != null)
                        {
                            downloadFinished(this, file);
                        }

                        return "OK";
                    }
                    else
                    {
                        LiveStatusMessage notif = notifMessage;
                        if (notif != null)
                        {
                            notif(this, FOLDER_NOT_FOUND, null);
                        }

                    }
                }
            }
            catch (TaskCanceledException)
            {
                LiveStatusMessage notif = notifMessage;
                if (notif != null)
                {
                    notif(this, TASK_CANCELLED, null);
                }
            }
            catch (Exception ex)
            {
                LiveStatusMessage notif = notifMessage;
                if (notif != null)
                {
                    notif(this, ERROR, ex);
                }

            }
            //finally
            //{
            //    resetTransfers();
            //}
            return "KO";
        }


    }

}
