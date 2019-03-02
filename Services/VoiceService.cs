using System;
using System.Globalization;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace UniversalKeepTheRhythm.Services
{
    class VoiceService : IDisposable
    {
        private static readonly object SyncRoot = new object();

        private static VoiceService _instance = null;
        public static VoiceService instance
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
                        _instance = new VoiceService();
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }

        }

        private MediaPlayer player;
        private SpeechSynthesizer text2speech;

        public CultureInfo SpeechLang;

        /*private void initVoice()
        {
            speechLang = CultureInfo.CurrentCulture;
            lang = CultureInfo.CurrentCulture.Name;

            text2speech = new SpeechSynthesizer();

            lang = text2speech.Voice.Language;

            string IdVoice = nowhereman.Properties.getProperty("languageVoice", "");
            if (IdVoice.Length > 0)
            {
                var voices = Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices;

                foreach (VoiceInformation t in voices)
                {
                    if (IdVoice == t.Id)
                    {
                        text2speech.Voice = t;
                        lang = t.Language;
                        break;
                    }
                }
            }

            if (!lang.Equals(speechLang.Name))
            {
                speechLang = new CultureInfo(lang);
            }
        }*/

        public VoiceService()
        {
            SpeechLang = CultureInfo.CurrentUICulture;
            string lang = CultureInfo.CurrentUICulture.Name;

            text2speech = new SpeechSynthesizer();

            lang = text2speech.Voice.Language;

            /* TODO 
            string IdVoice = nowhereman.Properties.getProperty("languageVoice", "");
            if (IdVoice.Length > 0)
            {
                // Query for a voice that speaks French.
                IEnumerable<VoiceInformation> frenchVoices = from voice in InstalledVoices.All
                                                             where voice.Id == IdVoice
                                                             select voice;
                foreach (var t in frenchVoices)
                {
                    text2speech.SetVoice(t);
                    lang = t.Language;
                    break;
                }
            }
            */
            if (!lang.Equals(SpeechLang.Name))
            {
                SpeechLang = new CultureInfo(lang);
            }

            this.player = new MediaPlayer();
            player.AutoPlay = true;
            player.AudioCategory = MediaPlayerAudioCategory.Alerts;
        }

        public async void Reader(string soundName, bool checkMusic, bool isLast = false)
        {

            //try
            //{
            //    if (task != null && task.Status == AsyncStatus.Started)
            //    {
            //        task.Cancel();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    nowhereman.LittleWatson.instance.Error("reader", ex);
            //}

            try
            {
                //if (isLast)
                //{
                //    // last should not be cancelled
                //    var task = text2speech.SpeakTextAsync(soundName);
                //}
                //else
                //{
                //    task = text2speech.SpeakTextAsync(soundName);
                //}

                SpeechSynthesisStream m_stream;

                if (soundName.StartsWith("<"))
                {
                    /*task =*/
                    m_stream = await text2speech.SynthesizeSsmlToStreamAsync(soundName);
                }
                else
                {
                    /*task =*/
                    m_stream = await text2speech.SynthesizeTextToStreamAsync(soundName);
                }


                player.Source = Windows.Media.Core.MediaSource.CreateFromStream(m_stream, m_stream.ContentType);
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult != 0x80045508)
                {
                    nowhereman.LittleWatson.instance.Error("speak", ex);
                }
            }

        }


        //private IAsyncAction task = null;

        //private void readerCancel()
        //{
        //    try
        //    {
        //        if (task != null) task.Cancel();
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        internal void Cancel()
        {
            try
            {
                player.Pause();
            }
            catch(Exception ex)
            {
                nowhereman.LittleWatson.instance.Error("cancel speak", ex);
            }

        }

        public void Dispose()
        {
            if (text2speech != null)
            {
                text2speech.Dispose();
            }

            if (player != null)
            {
                player.Dispose();
                player = null;
            }
        }
    }
}
