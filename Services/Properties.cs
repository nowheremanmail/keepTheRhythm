using System;
using System.Collections.Generic;

using System.Text;
using Windows.Storage;


namespace nowhereman
{
    public class Properties
    {

        static public bool exists(string name)
        {
            try
            {
                var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (applicationData.Values[name] != null)
                    return true;
                return false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
                return false;
            }
        }

        static public String getProperty(string name, string def)
        {
            try
            {
                object value;
                var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (applicationData.Values.TryGetValue(name, out value))
                {
                    return value.ToString();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
            }
            return def;
        }

        static public long getLongProperty(string name, long def)
        {
            try
            {
                object value;
                var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (applicationData.Values.TryGetValue(name, out value))
                {
                    return long.Parse(value.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
            }
            return def;
        }

        static public int getIntProperty(string name, int def)
        {
            try
            {
                object value;
                var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (applicationData.Values.TryGetValue(name, out value))
                {
                    return int.Parse(value.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
            }
            return def;
        }

        static public bool getBoolProperty(string name, bool def)
        {
            try
            {
                object value;
                var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (applicationData.Values.TryGetValue(name, out value))
                {
                    return bool.Parse(value.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
            }
            return def;
        }

        static public double getDoubleProperty(string name, double def)
        {
            try
            {
                object value;
                var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (applicationData.Values.TryGetValue(name, out value))
                {
                    return double.Parse(value.ToString());
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
            }
            return def;
        }

        static public void setBoolProperty(string name, bool value)
        {
            var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
            applicationData.Values[name] = value.ToString();
            // TODO IsolatedStorageSettings.ApplicationSettings.Save();
        }

        static public void setIntProperty(string name, int value)
        {
            var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
            applicationData.Values[name] = value.ToString();
        }

        static public void setLongProperty(string name, long value)
        {
            var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
            applicationData.Values[name] = value.ToString();
        }

        static public void removeProperty(string name)
        {
            var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
            applicationData.Values.Remove(name);
        }

        static public void setProperty(string name, string value)
        {
            var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
            applicationData.Values[name] = value;
        }

        static public void setDoubleProperty(string name, double value)
        {
            var applicationData = Windows.Storage.ApplicationData.Current.LocalSettings;
            applicationData.Values[name] = value.ToString();
        }

        //static public String getProperty(string name, string def)
        //{
        //    try
        //    {
        //        String value;
        //        if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(name, out value))
        //        {
        //            return value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
        //    }
        //    return def;
        //}
        //static public int getIntProperty(string name, int def)
        //{
        //    try
        //    {
        //        String value;
        //        if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(name, out value))
        //        {
        //            return int.Parse(value);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
        //    }
        //    return def;
        //}

        //static public bool getBoolProperty(string name, bool def)
        //{
        //    try
        //    {
        //        bool value;
        //        if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<bool>(name, out value))
        //        {
        //            return value;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine("error " + name + " " + e.Message);
        //    }
        //    return def;
        //}

        //static public void setBoolProperty(string name, bool value)
        //{
        //    IsolatedStorageSettings.ApplicationSettings.Remove(name);
        //    IsolatedStorageSettings.ApplicationSettings.Add(name, value);
        //    IsolatedStorageSettings.ApplicationSettings.Save();
        //}

        //static public void setIntProperty(string name, int value)
        //{
        //    IsolatedStorageSettings.ApplicationSettings.Remove(name);
        //    IsolatedStorageSettings.ApplicationSettings.Add(name, value.ToString());
        //    IsolatedStorageSettings.ApplicationSettings.Save();
        //}

        //static public void setProperty(string name, string value)
        //{
        //    IsolatedStorageSettings.ApplicationSettings.Remove(name);
        //    IsolatedStorageSettings.ApplicationSettings.Add(name, value);
        //    IsolatedStorageSettings.ApplicationSettings.Save();
        //}


    }
}
