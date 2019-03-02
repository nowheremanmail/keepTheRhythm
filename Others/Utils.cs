using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UniversalKeepTheRhythm.Others
{
    static public class Utils
    {
        static public string HtmlHeader(double viewportWidth, string extrastyle)
        {
            var head = new StringBuilder();
            try
            {
                head.Append("<head>");
                if (viewportWidth > 0)
                {
                    head.Append(string.Format(
                    "<meta name=\"viewport\" value=\"width={0}\" user-scalable=\"no\">",
                    viewportWidth));
                }
                head.Append("<style>");
                head.Append("html { -ms-text-size-adjust:150% }");
                head.Append(string.Format(
                "body {{background:{0};color:{1};font-family:'Segoe WP';font-size:2em;margin:0.5em;padding:0 }}",
                GetBrowserColor("ApplicationPageBackgroundThemeBrush"),
                GetBrowserColor("ApplicationForegroundThemeBrush")));
                head.Append(string.Format(
                "a {{color:{0}}}",
                GetBrowserColor("PhoneAccentColor")));
                if (extrastyle != null) head.Append(extrastyle);
                head.Append("img {width:80px;height:80px} </style>");
                //head.Append(NotifyScript);
                head.Append("</head>");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("HtmlHeader " + ex.Message);
            }
            return head.ToString();
        }

        internal static double fromDistanceString(string txt, string unit, CultureInfo currentCulture)
        {
            return ExtractDistance(txt); // TODO unit and culture
        }

        private static Dictionary<string, double> _DistanceLookup = new Dictionary<string, double>()
{
         {"mile", 1609.344},
  //{"furlong", 201.168},
  //{"yard", 0.9144},
  //{"inch", 0.0254},
  //{"foot", 0.3048},
  //{"feet", 0.3048},
  {"kilometer", 1000},
  {"kilometre", 1000},
  {"metre", 1},
  {"meter", 1},
  {"km", 1000},
  {"m", 1}
  //{"centimeter", 0.01},
  //{"centimetre", 0.01},
  //{"millimeter", 0.001},
  //{"millimetre", 0.001},
};

        private static double ConvertFraction(string fraction)
        {
            double value = 0;
            if (fraction.Contains("/"))
            {
                // If the value contains /, we need to work out the fraction
                string[] splitVal = fraction.Split('/');
                if (splitVal.Length != 2)
                {
                    ScrewUp(fraction, "splitVal.Length");
                }

                // Turn the fraction into decimal
                value = double.Parse(splitVal[0]) / double.Parse(splitVal[1]);
            }
            else
            {
                // Otherwise it's a simple parse
                value = double.Parse(fraction);
            }
            return value;
        }

        public static double ExtractDistance(string distAsString)
        {
            double distanceInMeters = 0;
            /* This will have a match per unit type.
             * e.g., the string "1 1/16 Miles 3/4 Yards" would have 2 matches
             * being "1 1/16 Miles", "3/4 Yards".  Each match will then have 4
             * groups in total, with group 3 being the raw value and 4 being the
             * raw unit
             */
            var matches = Regex.Matches(distAsString, @"(([\d]+[\d\s\.,/]*)\s([A-Za-z]+[^\s\d]))");
            foreach (Match match in matches)
            {
                // If groups != 4 something went wrong, we need to rethink our regex
                if (match.Groups.Count != 4)
                {
                    ScrewUp(distAsString, "match.Groups.Count");
                }
                string valueRaw = match.Groups[2].Value;
                string unitRaw = match.Groups[3].Value;

                // Firstly get the value
                double value = 0;
                if (valueRaw.Contains(" "))
                {
                    // If the value contains /, we need to work out the fraction
                    string[] splitVal = valueRaw.Split(' ');
                    if (splitVal.Length != 2)
                    {
                        ScrewUp(distAsString, "splitVal.Length");
                    }

                    // Turn the fraction into decimal
                    value = ConvertFraction(splitVal[0]) + ConvertFraction(splitVal[1]);
                }
                else
                {
                    value = ConvertFraction(valueRaw);
                }

                // Now work out based on the unit type
                // Clean up the raw unit string
                unitRaw = unitRaw.ToLower().Trim().TrimEnd('s');

                if (!_DistanceLookup.ContainsKey(unitRaw))
                {
                    ScrewUp(distAsString, "unitRaw");
                }
                distanceInMeters += value * _DistanceLookup[unitRaw];
            }
            return distanceInMeters;
        }

        private static void ScrewUp(string val, string prop)
        {
            throw new ArgumentException("Extract distance screwed up on string [" + val + "] (bad " + prop + ")");
        }

        private static string GetBrowserColor(string sourceResource)
        {
            try
            {
                var color = ((SolidColorBrush)Application.Current.Resources[sourceResource]);
                return "#" + color.Color.ToString().Substring(3, 6);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("NOT FOUND " + sourceResource + " " + ex.Message);
                return "#010101";
            }
        }
        static async public Task<Uri> WrapHtml(string htmlSubString, double viewportWidth, string extraStyle, string[] files)
        {
            try
            {

                if (files != null)
                {
                    foreach (string f in files)
                    {
                        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + f));

                        StorageFolder folder = ApplicationData.Current.LocalFolder;
                        var tmp = f.Split('/');
                        var name = f;
                        if (tmp.Length > 1)
                        {
                            try
                            {
                                folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(tmp[0]);
                            }catch (Exception )
                            {
                                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(tmp[0], CreationCollisionOption.ReplaceExisting);
                            }
                            name = tmp[1];
                        }

                        var x = await file.CopyAsync(folder, name, NameCollisionOption.ReplaceExisting);
                        if (x != null)
                        {

                        }
                    }

                }

                var dest = await ApplicationData.Current.LocalFolder.CreateFileAsync("help.html", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(dest, WrapHtml(htmlSubString, viewportWidth, extraStyle));

                return new Uri("ms-appdata:///local/help.html");
            }
            catch (Exception ex)
            {
                nowhereman.LittleWatson.instance.ReportException(ex, "copying images");
            }
            return null;
        }


        static async private void SaveFilesToIsoStore(string[] files)
        {

            //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///help.html"));
            //StorageFolder folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("anyFolder", CreationCollisionOption.OpenIfExists);
            //await file.CopyAsync(folder, "help.html", NameCollisionOption.ReplaceExisting);
            //webView.Navigate(new Uri("ms-appdata:///local/anyFolder/help.html"));

            //These files must match what is included in the application package,
            //or BinaryStream.Dispose below will throw an exception.

        }

        static public string WrapHtml(string htmlSubString, double viewportWidth, string extraStyle)
        {

            var html = new StringBuilder();
            html.Append("<html>");
            html.Append(HtmlHeader(viewportWidth, extraStyle));
            html.Append("<body>");
            html.Append(htmlSubString);
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }

        static public string toPaceString(double pace, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                return Math.Round(pace, 1).ToString("0.0", inf) + " min/km";
            }
            else
            {
                // 1.609344 km -> 1 mile
                return Math.Round(pace * 1.609344, 1).ToString("0.0", inf) + " mph";
            }
        }

        static public string toPaceStringShort(double pace, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                return Math.Round(pace, 1).ToString("0.0", inf);
            }
            else
            {
                // 1.609344 km -> 1 mile
                return Math.Round(pace * 1.609344, 1).ToString("0.0", inf);
            }
        }
        static public string toDistanceString(double distance, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                if (distance < 1000)
                {
                    return distance.ToString("0") + " m";
                }
                if (distance < 1000000)
                {
                    return Math.Round(distance / 1000.0, 1).ToString("0.0", inf) + " km";
                }

                return Math.Round(distance / 1000.0, 1).ToString("0", inf) + " km";
            }
            else
            {
                // 1m   1 km       1 mile
                //    -----------------------
                //     1000 m     1.609344 km
                double _distance = distance / 1609.344;

                if (_distance < 1)
                {
                    // 1m   1 yard   
                    //    ----------
                    //     0.9144 m    
                    return Math.Round(distance / 0.9144, 0).ToString("0") + " ya";
                }
                return Math.Round(_distance, 1).ToString("0.0", inf) + " mi";
            }
        }
        static public string toDistanceStringShort(double distance, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                if (distance < 1000)
                {
                    return distance.ToString("0");
                }
                if (distance < 1000000)
                {
                    return Math.Round(distance / 1000.0, 1).ToString("0.0", inf);
                }
                return Math.Round(distance / 1000.0, 1).ToString("0", inf);
            }
            else
            {
                // 1m   1 km       1 mile
                //    -----------------------
                //     1000 m     1.609344 km
                double _distance = distance / 1609.344;

                if (_distance < 1)
                {
                    // 1m   1 yard   
                    //    ----------
                    //     0.9144 m    
                    return Math.Round(distance / 0.9144, 0).ToString("0");
                }
                return Math.Round(_distance, 1).ToString("0.0", inf);
            }
        }
        static public string toTimeStringF(long p)
        {
            TimeSpan tmp = new TimeSpan(p);
            return toTimeStringF(tmp);
        }

        static public string toTimeStringL(long p)
        {
            TimeSpan tmp = new TimeSpan(p);
            return toTimeStringL(tmp);
        }

        static public string toTimeString(long p)
        {
            TimeSpan tmp = new TimeSpan(p);
            return toTimeString(tmp);
        }

        static public string toReaderDistance(double d, string unit, ResourceLoader resourceLoader, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                int mm = (int)Math.Round(d, 0);
                int km = mm / 1000;
                mm = mm % 1000;

                if (km < 1000)
                {
                    if (mm == 1 && km == 0)
                        return string.Format(resourceLoader.GetString("oneMeter"/* TODO , inf*/), mm.ToString());
                    else if (mm < 1000 && km == 0)
                        return string.Format(resourceLoader.GetString("oneMeters"/* TODO , inf*/), mm.ToString());
                    else if (mm == 1 && km == 1)
                        return string.Format(resourceLoader.GetString("oneKmM"/* TODO , inf*/), km.ToString(), mm.ToString());
                    else if (mm == 0 && km == 1)
                        return string.Format(resourceLoader.GetString("oneKilometre"/* TODO , inf*/), km.ToString(), mm.ToString());
                    else if (mm > 1 && km == 1)
                        return string.Format(resourceLoader.GetString("oneKmMs"/* TODO , inf*/), km.ToString(), mm.ToString());
                    else if (mm == 1 && km > 1)
                        return string.Format(resourceLoader.GetString("oneKmsM"/* TODO , inf*/), km.ToString(), mm.ToString());
                    else if (mm == 0 && km > 1)
                        return string.Format(resourceLoader.GetString("oneKilometres"/* TODO , inf*/), km.ToString(), mm.ToString());

                    return string.Format(resourceLoader.GetString("oneKmsMs"/* TODO , inf*/), km.ToString(), mm.ToString());
                }
                else
                {
                    return string.Format(resourceLoader.GetString("oneKilometres"/* TODO , inf*/), km.ToString("0E0"));
                }
            }
            else
            {
                double m = (d / 1609.344);

                if (m == 0)
                    return string.Format(resourceLoader.GetString("unitMile"/* TODO , inf*/), m.ToString("0.0"/* TODO , inf*/));
                else
                    return string.Format(resourceLoader.GetString("unitMiles"/* TODO , inf*/), m.ToString("0.0"/* TODO , inf*/));
            }
        }

        static public string toReaderTime(TimeSpan tmp, ResourceLoader resourceLoader, CultureInfo inf)
        {
            if (tmp.Hours < 1)
            {
                if (tmp.Minutes == 1)
                {
                    return string.Format(resourceLoader.GetString("oneMinute"/* TODO , inf*/), 1);
                }
                else
                {
                    return string.Format(resourceLoader.GetString("oneMinutes"/* TODO , inf*/), tmp.Minutes);
                }
            }
            else if (tmp.Hours == 1)
            {
                if (tmp.Minutes == 0)
                {
                    return string.Format(resourceLoader.GetString("oneHour0Minute"/* TODO , inf*/), 1, tmp.Minutes);
                }
                else if (tmp.Minutes == 1)
                {
                    return string.Format(resourceLoader.GetString("oneHourMinute"/* TODO , inf*/), 1, tmp.Minutes);
                }
                else
                {
                    return string.Format(resourceLoader.GetString("oneHourMinutes"/* TODO , inf*/), 1, tmp.Minutes);
                }
            }
            else
            {
                if (tmp.Minutes == 0)
                {
                    return string.Format(resourceLoader.GetString("oneHours0Minute"/* TODO , inf*/), tmp.Hours, tmp.Minutes);
                }
                else if (tmp.Minutes == 1)
                {
                    return string.Format(resourceLoader.GetString("oneHoursMinute"/* TODO , inf*/), tmp.Hours, tmp.Minutes);
                }
                else
                {
                    return string.Format(resourceLoader.GetString("oneHoursMinutes"/* TODO , inf*/), tmp.Hours, tmp.Minutes);
                }
            }
        }

        static public string toTimeStringL(TimeSpan tmp)
        {
            return tmp.ToString("d'd 'h'h 'mm'm'");
        }

        static public string toTimeString(TimeSpan tmp)
        {
            return tmp.ToString("h'h 'mm'm'");
        }

        static public string toTimeStringF(TimeSpan tmp)
        {
            return tmp.ToString("hh':'mm':'ss");
        }

        static public double toKmh(double a, string unit)
        {
            if ("m".Equals(unit))
            {
                return a * 3.6;
            }
            else
            {
                return (a * 3.6) / 1.609344;
            }
        }

        static public string toKmhStringShort(double p, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                return toKmh(p, unit).ToString("0.0", inf);
            }
            else
            {
                // 1.609344 km -> 1 mile
                // 1m   1 km       1 mile         3600s
                //---    ------------------------------
                //  s   1000 m     1.609344 km      1h
                return toKmh(p, unit).ToString("0.0", inf); //CultureInfo.CurrentCulture);
            }
        }

        static public string toKmhString(double p, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                return toKmh(p, unit).ToString("0.0", inf) + " km/h";
            }
            else
            {
                // 1.609344 km -> 1 mile
                // 1m   1 km       1 mile         3600s
                //---    ------------------------------
                //  s   1000 m     1.609344 km      1h
                return toKmh(p, unit).ToString("0.0", inf) + " mph";
            }
        }

        static public string toAltitudeStringShort(double p, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                if (Math.Abs(p) < 1000000)
                {
                    return toAltitude(p, unit).ToString("0");
                }
                return toAltitude(p, unit).ToString("0E0");
            }
            else
            {
                // 1.609344 km -> 1 mile
                // 1m   1 km       1 mile
                //    -----------------------
                //     1000 m     1.609344 km

                return toAltitude(p, unit).ToString("0.0", inf);
            }
        }

        static public string toAltitudeString(double p, string unit, CultureInfo inf)
        {
            if ("m".Equals(unit))
            {
                if (Math.Abs(p) < 1000000)
                {
                    return toAltitude(p, unit).ToString("0") + " m";
                }
                else
                    return toAltitude(p, unit).ToString("0E0") + " m";
            }
            else
            {
                // 1.609344 km -> 1 mile
                // 1m   1 km       1 mile
                //    -----------------------
                //     1000 m     1.609344 km

                return toAltitude(p, unit).ToString("0.0", inf) + " mi";
            }
        }

        static public double toAltitude(double p, string unit)
        {
            if ("m".Equals(unit))
            {
                return Math.Round(p, 0);
            }
            else
            {
                // 1.609344 km -> 1 mile
                // 1m   1 km       1 mile
                //    -----------------------
                //     1000 m     1.609344 km

                return (p / 1609.344);
            }
        }

        static public double toPace(TimeSpan t, double currentDistance)
        {
            if (currentDistance > 0)
            {
                return t.TotalMinutes * 1000 / currentDistance;
            }
            else
            {
                return 0;
            }
        }

        static public double toPace(long currentTime, double currentDistance)
        {
            TimeSpan t = new TimeSpan(currentTime);
            if (currentDistance > 0)
            {
                return t.TotalMinutes * 1000 / currentDistance;
            }
            else
            {
                return 0;
            }
        }

        static public double toPaceSecMet(long currentTime, double currentDistance)
        {
            if (currentDistance > 0)
            {
                return (currentTime * 1000) / (currentDistance * 60);
            }
            else
            {
                return 0;
            }
        }

        static public double MaxTime(string mode)
        {
            //return MaxSpeed("C") / MaxSpeed(mode);

            if ("W".Equals(mode))
            {
                return 5;
            }
            else if ("R".Equals(mode))
            {
                return 2.5;
            }
            else
            {
                return 1;
            }
        }

        static public double MaxSpeed(string mode)
        {
            if ("W".Equals(mode))
            {
                return 9000.0 / 3600.0;
            }
            else if ("R".Equals(mode))
            {
                return 100.0 / 8.0;
            }
            else
            {
                //                             if ("C".Equals (mode)) 
                return 50000.0 / 3600.0;
            }
        }

        static public string NOW()
        {
            return DateTime.Now.ToString("yyyy'-'MM'-'dd HH':'mm", CultureInfo.InvariantCulture);
        }

        static public DateTime _FromNOW(string d)
        {
            return DateTime.ParseExact(d, "yyyy'-'MM'-'dd HH':'mm", CultureInfo.InvariantCulture);
        }

        static public string FromNOW(string d)
        {
            return DateTime.ParseExact(d, "yyyy'-'MM'-'dd HH':'mm", CultureInfo.InvariantCulture).ToString("d", CultureInfo.CurrentCulture.DateTimeFormat);
        }

        static public string FromNOWLong(string d)
        {
            return DateTime.ParseExact(d, "yyyy'-'MM'-'dd HH':'mm", CultureInfo.InvariantCulture).ToString("g", CultureInfo.CurrentCulture.DateTimeFormat);
        }

        public static Color getColor(double res, double margin)
        {
            Color col = Colors.Green;
            int signRes = 0;
            if (Math.Abs(res) >= margin)
                signRes = Math.Sign(res) * 2;
            else
                if (Math.Abs(res) >= (margin / 2))
                signRes = Math.Sign(res);


            if (Math.Abs(signRes) >= 2)
            {
                if (signRes > 0) // slow
                    col = Colors.Red;
                else
                    if (signRes < 0)
                    col = Colors.Blue;
            }
            else
            {
                if (signRes > 0)
                    col = Colors.Orange;
                else
                    if (signRes < 0)
                    col = Colors.Purple;
            }
            return col;
        }

    }

}
