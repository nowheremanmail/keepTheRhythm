using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.UI.Popups;

namespace nowhereman
{
    public class LittleWatson
    {
        const string filename = "LittleWatson.txt";

        private static LittleWatson _instance = null;
        public static LittleWatson instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LittleWatson();
                }
                return _instance;

            }
        }


        public void Error(string msg, Exception e)
        {
            try
            {
                ReportException(e, msg);
            }
            catch { }
            ////#if(DEBUG)
            //            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //            {
            //                MessageBox.Show(msg + " " + e.Message);
            //            }
            //            );
            ////#endif
        }



        private static string Feedback()
        {
            Windows.Storage.StorageFolder installedLocation = Package.Current.InstalledLocation;

            Package package = Package.Current;
            PackageId packageId = package.Id;

            string version = versionString(packageId.Version);

            //            String output = String.Format("Name: \"{0}\"\n" +
            //                                          "Version: {1}\n" +
            //                                          "Architecture: {2}\n" +
            //                                          "ResourceId: \"{3}\"\n" +
            //                                          "Publisher: \"{4}\"\n" +
            //                                          "PublisherId: \"{5}\"\n" +
            //                                          "FullName: \"{6}\"\n" +
            //                                          "FamilyName: \"{7}\"\n" +
            //                                          "IsFramework: {8}\n",
            //                                          packageId.Name,
            //                                          versionString(packageId.Version),
            //                                          architectureString(packageId.Architecture),
            //                                          packageId.ResourceId,
            //                                          packageId.Publisher,
            //                                          packageId.PublisherId,
            //                                          packageId.FullName,
            //                                          packageId.FamilyName,
            //                                          package.IsFramework);
            //#if WINDOWS_APP
            //            output += String.Format("IsResourcePackage: {0}\n" +
            //                                    "IsBundle: {1}\n" +
            //                                    "IsDevelopmentMode: {2}\n" +
            //                                    "DisplayName: \"{3}\"\n" +
            //                                    "PublisherDisplayName: \"{4}\"\n" +
            //                                    "Description: \"{5}\"\n" +
            //                                    "Logo: \"{6}\"\n",
            //                                    package.IsResourcePackage,
            //                                    package.IsBundle,
            //                                    package.IsDevelopmentMode,
            //                                    package.DisplayName,
            //                                    package.PublisherDisplayName,
            //                                    package.Description,
            //                                    package.Logo.AbsoluteUri);
            //#endif
            var deviceInformation = new EasClientDeviceInformation();
            var deviceName = deviceInformation.FriendlyName;
            var operatingSystem = deviceInformation.OperatingSystem;

            return string.Format(ResourceLoader.GetForCurrentView().GetString("SupportBody"),
                 deviceName,
                 "", //packageId.FamilyName, // MANufactor
                 operatingSystem, // firmware version
                 architectureString(packageId.Architecture), // hardware version
                 version,
                 "no where man");
        }

        static string versionString(PackageVersion version)
        {
            return String.Format("{0}.{1}.{2}.{3}",
                                 version.Major, version.Minor, version.Build, version.Revision);
        }

        static string architectureString(Windows.System.ProcessorArchitecture architecture)
        {
            switch (architecture)
            {
                case Windows.System.ProcessorArchitecture.X86:
                    return "x86";
                case Windows.System.ProcessorArchitecture.Arm:
                    return "arm";
                case Windows.System.ProcessorArchitecture.X64:
                    return "x64";
                case Windows.System.ProcessorArchitecture.Neutral:
                    return "neutral";
                case Windows.System.ProcessorArchitecture.Unknown:
                    return "unknown";
                default:
                    return "???";
            }
        }


        public void ReportException(Exception ex, string extra)
        {
            Task.Run(async () =>
            {
                try
                {
                    StorageFolder folder = ApplicationData.Current.TemporaryFolder;
                    using (Stream strem = await folder.OpenStreamForWriteAsync(filename, CreationCollisionOption.OpenIfExists))
                    {

                        if (strem.Length > 5 * 1024)
                        {
                            strem.SetLength(0);
                        }

                        using (TextWriter output = new StreamWriter(strem))
                        {
                            output.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            output.WriteLine(DateTime.Now.ToUniversalTime());
                            output.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                            output.WriteLine(extra);
                            if (ex != null)
                            {
                                output.WriteLine(ex.Message);
                                output.WriteLine(ex.StackTrace);
                            }
                            output.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            output.WriteLine("");
                        }
                    }
                }

                catch (Exception)
                {
                }
            });
        }

        public async void CheckForPreviousExceptionAsync(string _emailTo, string _subject, string content, string title)
        {
            StorageFile file = null;
            try
            {
                string contents = null;
                try
                {
                    file = await Windows.Storage.ApplicationData.Current.TemporaryFolder.GetFileAsync(filename);


                    contents = await FileIO.ReadTextAsync(file);
                }
                catch (FileNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("no error file");
                }
                if (contents != null && contents.Length > 0)
                {
                    contents += Feedback();

                    MessageDialog result = new MessageDialog(content, title);
                    result.Commands.Add(new UICommand(ResourceLoader.GetForCurrentView().GetString("dialog_Yes"), async (command) =>
                    {
                        try
                        {
                            //EmailMessage em = new EmailMessage();

                            //em.To.Add(new EmailRecipient(_emailTo));
                            //em.Subject = _subject;
                            //em.Body = contents;
                            //// You can add an attachment that way.
                            ////em.Attachments.Add(new EmailAttachment(...);

                            //// Show the email composer.
                            //await EmailManager.ShowComposeNewEmailAsync(em);

                            Uri uri = new Uri("mailto:?to=" + _emailTo + "&subject=" + _subject + "&body=" + contents);

                            if (uri != null)
                            {
                                var launched = await Windows.System.Launcher.LaunchUriAsync(uri);
                            }
                        }
                        catch (Exception)
                        {
                            //handle the exception
                        }
                    }));
                    result.Commands.Add(new UICommand(ResourceLoader.GetForCurrentView().GetString("dialog_No"), (command) =>
                    {

                    })); await result.ShowAsync();

                }

                if (file != null)
                {
                    await file.DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("error ?? " + ex.Message);
            }
            finally
            {
            }
        }

    }

}
