using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UniversalKeepTheRhythm.model
{
    public class Settings : INotifyPropertyChanged
    {
        private static readonly object SyncRoot = new object();

        private static Settings _instance = null;

        static public Settings instance()
        {
            if (_instance != null)
            {
                return _instance;
            }


            lock (SyncRoot)
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                    _instance.prepare();
                }
            }

            return _instance;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        private ElementTheme appTheme;
        public bool whiteTheme
        {
            get
            {
                return appTheme == ElementTheme.Light;
            }
            set
            {
                AppTheme = ElementTheme.Light;
            }
        }

        public bool blackTheme
        {
            get
            {
                return appTheme == ElementTheme.Dark;
            }
            set
            {
                AppTheme = ElementTheme.Dark;
            }
        }

        public bool defaultTheme
        {
            get
            {
                return appTheme == ElementTheme.Default;
            }
            set
            {
                AppTheme = ElementTheme.Default;
            }
        }

        public ElementTheme AppTheme
        {
            get
            {
                return this.appTheme;
            }
            set
            {
                if (this.appTheme != value)
                {
                    this.appTheme = value;
                    NotifyPropertyChanged("whiteTheme");
                    NotifyPropertyChanged("blackTheme");
                    NotifyPropertyChanged("defaultTheme");
                    NotifyPropertyChanged("AppTheme");
                }
            }

        }


        private void prepare()
        {

            this.appTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), nowhereman.Properties.getProperty("AppTheme", ElementTheme.Default.ToString()), true);
        }

        }
    }
