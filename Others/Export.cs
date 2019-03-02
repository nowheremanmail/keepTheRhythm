using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UniversalKeepTheRhythm.model;
using UniversalKeepTheRhythm.Services;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace UniversalKeepTheRhythm.Others
{
    public class ExportImport
    {
        private static char[] sep3 = { '|' };

        public static async Task<StorageFile> exportKml(int idSession, string name, string unit, ResourceLoader resourceLoader)
        {
            var cult = CultureInfo.CurrentUICulture;
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

            using (var isoFileStream = await file.OpenStreamForWriteAsync())
            {
                XmlWriterSettings wSettings = new XmlWriterSettings();
                wSettings.Indent = true;

                XmlWriter xw = XmlWriter.Create(isoFileStream, wSettings);// Write Declaration
                xw.WriteStartDocument();

                xw.WriteStartElement("kml", "http://www.opengis.net/kml/2.2");
                xw.WriteAttributeString("xmlns", "gx", null, "http://www.google.com/kml/ext/2.2");
                xw.WriteAttributeString("xmlns", "kml", null, "http://www.opengis.net/kml/2.2");
                xw.WriteAttributeString("xmlns", "atom", null, "http://www.w3.org/2005/Atom");

                var session = DataBaseManager.instance.GetSession(idSession);
                var path = DataBaseManager.instance.GetPath(session.IdPath);

                /*xw.WriteStartElement("time");
                xw.WriteString(session.dayOfSession);
                xw.WriteEndElement();*/

                //DateTime start = DateTime.Parse(session.dayOfSession);
                //long startId = start.Ticks;

                xw.WriteStartElement("Document");
                xw.WriteStartElement("name");
                xw.WriteString(string.Format(resourceLoader.GetString("NameKml"), path.Description, Utils.FromNOW(session.DayOfSession)));
                xw.WriteEndElement(); // name

                string desc = "<div class=\"summary\"><div><span class=\"distance\">Distance</span><span>" + Utils.toDistanceString(session.Distance.HasValue ? session.Distance.Value : 0, unit, cult) + "</span></div>"
                + "<div><span class=\"time\">Time</span><span>" + Utils.toTimeString(session.Duration.HasValue ? session.Duration.Value : 0) + "</span></div>"

                + "<div><span class=\"pace\">Pace</span><span>" + Utils.toPaceString(session.ObjTime.HasValue && session.ObjDistance.HasValue ? Utils.toPaceSecMet(session.ObjTime.Value, session.ObjDistance.Value) : 0, unit, cult) + "</span></div>"
                + "<div><span class=\"plannedPace\">Planned Pace</span><span>" + Utils.toPaceString(session.AvgPace.HasValue ? session.AvgPace.Value : 0, unit, cult) + "</span></div>"

                 + "<div><span class=\"speed\">Speed</span><span>" + string.Format("{0} .. {1}", Utils.toKmhStringShort(session.MinSpeed.HasValue ? session.MinSpeed.Value : 0, unit, CultureInfo.CurrentCulture), Utils.toKmhString(session.MaxSpeed.HasValue ? session.MaxSpeed.Value : 0, unit, CultureInfo.CurrentCulture)) + "</span></div>"
                 + "<div><span class=\"avgSpeed\">Avg Speed</span><span>" + Utils.toKmhString(session.Distance.Value / TimeSpan.FromTicks(session.Duration.Value).TotalSeconds, unit, CultureInfo.CurrentCulture) + "</span></div>"

                 + "<div><span class=\"altitude\">Altitude</span><span>" + string.Format("{0} .. {1}", Utils.toAltitudeStringShort(session.MinAltitude.HasValue ? session.MinAltitude.Value : 0, unit, CultureInfo.CurrentCulture), Utils.toAltitudeString(session.MaxAltitude.HasValue ? session.MaxAltitude.Value : 0, unit, CultureInfo.CurrentCulture)) + "</span></div>"
                 + "<div><span class=\"avgAltitude\">Avg altitude</span><span>" + Utils.toAltitudeString(session.AvgAltitude.HasValue ? session.AvgAltitude.Value : 0, unit, CultureInfo.CurrentCulture) + "</span></div>"
                + "<div><span class=\"ascendent\">Ascendent</span><span>" + Utils.toAltitudeString(session.Ascendent.HasValue ? session.Ascendent.Value : 0, unit, cult) + "</span></div>"
                + "<div><span class=\"descendent\">Descendent</span><span>" + Utils.toAltitudeString(session.Descendent.HasValue ? session.Descendent.Value : 0, unit, cult) + "</span></div>"

                + "</div>";

                xw.WriteStartElement("description");
                xw.WriteCData(desc);
                xw.WriteEndElement(); // name

                xw.WriteStartElement("Style");
                xw.WriteStartAttribute("id");
                xw.WriteString("yellowLineGreenPoly");
                xw.WriteEndAttribute();

                xw.WriteStartElement("LineStyle");
                xw.WriteStartElement("color");
                xw.WriteString("7f00ffff");
                xw.WriteEndElement();

                xw.WriteStartElement("width");
                xw.WriteString("4");
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteStartElement("PolyStyle");
                xw.WriteStartElement("color");
                xw.WriteString("7f00ff00");
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteStartElement("Style");
                xw.WriteStartAttribute("id");
                xw.WriteString("downArrowIcon");
                xw.WriteEndAttribute();
                xw.WriteStartElement("BalloonStyle");
                xw.WriteStartElement("textColor");
                xw.WriteString("99ffa000");
                xw.WriteEndElement();
                xw.WriteStartElement("text");
                xw.WriteCData("<div class=\"divMusic\"><div class=\"name\">$[name]</div>$[description]</div>");
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteStartElement("Style");
                xw.WriteStartAttribute("id");
                xw.WriteString("downArrowIconEnd");
                xw.WriteEndAttribute();
                xw.WriteStartElement("BalloonStyle");
                xw.WriteStartElement("textColor");
                xw.WriteString("ff000000");
                xw.WriteEndElement();
                xw.WriteStartElement("text");
                xw.WriteCData("<div class=\"divEnd\"><div class=\"name\">$[name]</div>$[description]</div>");
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteStartElement("Style");
                xw.WriteStartAttribute("id");
                xw.WriteString("downArrowIconStart");
                xw.WriteEndAttribute();
                xw.WriteStartElement("BalloonStyle");
                xw.WriteStartElement("textColor");
                xw.WriteString("00FF0000");
                xw.WriteEndElement();
                xw.WriteStartElement("text");
                xw.WriteCData("<div class=\"divStart\"><div class=\"name\">$[name]</div>$[description]</div>");
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteStartElement("Style");
                xw.WriteStartAttribute("id");
                xw.WriteString("downArrowIconCenter");
                xw.WriteEndAttribute();
                xw.WriteStartElement("BalloonStyle");
                xw.WriteStartElement("textColor");
                xw.WriteString("00FF0000");
                xw.WriteEndElement();
                xw.WriteStartElement("text");
                xw.WriteCData("<div class=\"divCenter\"><div class=\"name\">$[name]</div>$[description]</div>");
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteStartElement("Folder");
                foreach (PointInterest tmp in DataBaseManager.instance.GetPoints(idSession))
                {
                    if (Constants.HIGHER.Equals(tmp.Message) || Constants.LOWER.Equals(tmp.Message) || tmp.Message.StartsWith(Constants.EXTRA_POINTS))
                    {
                        continue;
                    }

                    if (!Double.IsNaN(tmp.Longitude) && !Double.IsNaN(tmp.Latitude))
                    {
                        if (tmp.Message.StartsWith(Constants.INFO_POINTS))
                        {
                            xw.WriteStartElement("Placemark");
                            xw.WriteStartElement("name");
                            xw.WriteString(tmp.Distance.ToString("0", CultureInfo.InvariantCulture.NumberFormat) + "|" + TimeSpan.FromTicks(tmp.Time).ToString("hh':'mm':'ss", CultureInfo.InvariantCulture));
                            //xw.WriteString(string.Format(AppResources.DetailKml, Utils.toDistanceString(tmp.distance, CultureInfo.CurrentCulture), Utils.toTimeString(tmp.time)));
                            xw.WriteEndElement();
                            xw.WriteStartElement("description");

                            xw.WriteString(tmp.Message.Replace("'", " ").Replace("\"", " "));

                            xw.WriteEndElement();
                            xw.WriteStartElement("styleUrl");
                            xw.WriteString("#downArrowIcon");
                            xw.WriteEndElement();
                            xw.WriteStartElement("Point");
                            xw.WriteStartElement("coordinates");
                            xw.WriteString(tmp.Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + tmp.Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0");
                            xw.WriteEndElement();
                            xw.WriteEndElement();
                            xw.WriteEndElement();
                        }
                        else
                        {
                            xw.WriteStartElement("Placemark");
                            xw.WriteStartElement("name");
                            xw.WriteString(tmp.Distance.ToString("0", CultureInfo.InvariantCulture.NumberFormat) + "|" + TimeSpan.FromTicks(tmp.Time).ToString("hh':'mm':'ss", CultureInfo.InvariantCulture));
                            //xw.WriteString(string.Format(AppResources.DetailKml, Utils.toDistanceString(tmp.distance, CultureInfo.CurrentCulture), Utils.toTimeString(tmp.time)));
                            xw.WriteEndElement();
                            xw.WriteStartElement("description");

                            string[] v = tmp.Message.Split(sep3);
                            if (v.Length == 3)
                            {
                                xw.WriteCData("<div itemprop=\"track\" itemscope itemtype=\"http://schema.org/MusicRecording\"><span itemprop=\"name\">" + v[2] + "</span><span itemprop=\"byArtist\">" + v[0] + "</span><meta content=\"" + v[1] + "\" itemprop=\"inAlbum\" /></div>");
                            }
                            else
                            {
                                xw.WriteString(tmp.Message.Replace("'", " ").Replace("\"", " "));
                            }

                            xw.WriteEndElement();
                            xw.WriteStartElement("styleUrl");
                            xw.WriteString("#downArrowIcon");
                            xw.WriteEndElement();
                            xw.WriteStartElement("Point");
                            xw.WriteStartElement("coordinates");
                            xw.WriteString(tmp.Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + tmp.Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0");
                            xw.WriteEndElement();
                            xw.WriteEndElement();
                            xw.WriteEndElement();
                        }
                    }
                }
                //var sessions = getMesures(idSession);

                //foreach (Mesures tmp in sessions)
                //{
                //    if (tmp.action != null && Constants.LOOP_DETECTED.Equals(tmp.action))
                //    {
                //        xw.WriteStartElement("Placemark");
                //        xw.WriteStartElement("name");
                //        xw.WriteString(tmp.distance.ToString("0", CultureInfo.InvariantCulture.NumberFormat) + "|" + TimeSpan.FromTicks(tmp.time).ToString("hh':'mm':'ss", CultureInfo.InvariantCulture));
                //        //xw.WriteString(string.Format(AppResources.DetailKml, Utils.toDistanceString(tmp.distance, CultureInfo.CurrentCulture), Utils.toTimeString(tmp.time)));
                //        xw.WriteEndElement();
                //        xw.WriteStartElement("description");

                //        string[] v = tmp.message.Split(sep3);
                //        if (v.Length == 3)
                //        {
                //            xw.WriteCData("<div itemprop=\"track\" itemscope itemtype=\"http://schema.org/MusicRecording\"><span itemprop=\"name\">" + v[2] + "</span><span itemprop=\"byArtist\">" + v[0] + "</span><meta content=\"" + v[1] + "\" itemprop=\"inAlbum\" /></div>");
                //        }
                //        else
                //        {
                //            xw.WriteString(tmp.message.Replace("'", " ").Replace("\"", " "));
                //        }

                //        xw.WriteEndElement();
                //        xw.WriteStartElement("styleUrl");
                //        xw.WriteString("#downArrowIcon");
                //        xw.WriteEndElement();
                //        xw.WriteStartElement("Point");
                //        xw.WriteStartElement("coordinates");
                //        xw.WriteString(tmp.longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + tmp.latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0");
                //        xw.WriteEndElement();
                //        xw.WriteEndElement();
                //        xw.WriteEndElement();
                //    }
                //}

                if (session.CenterLat.HasValue && session.CenterLon.HasValue && !double.IsNaN(session.CenterLat.Value) && !double.IsNaN(session.CenterLon.Value))
                {
                    xw.WriteStartElement("Placemark");
                    xw.WriteStartElement("name");
                    xw.WriteString(string.Format(resourceLoader.GetString("NameKml"), path.Description, Utils.FromNOW(session.DayOfSession)));
                    xw.WriteEndElement();
                    xw.WriteStartElement("description");
                    xw.WriteCData(desc);
                    xw.WriteEndElement();
                    xw.WriteStartElement("styleUrl");
                    xw.WriteString("#downArrowIconCenter");
                    xw.WriteEndElement();
                    xw.WriteStartElement("Point");
                    xw.WriteStartElement("coordinates");
                    xw.WriteString(session.CenterLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + session.CenterLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0");
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                if (session.StartLat.HasValue && session.StartLon.HasValue && !double.IsNaN(session.StartLat.Value) && !double.IsNaN(session.StartLon.Value))
                {
                    xw.WriteStartElement("Placemark");
                    xw.WriteStartElement("name");
                    xw.WriteString(string.Format(resourceLoader.GetString("NameKml"), path.Description, Utils.FromNOW(session.DayOfSession)));
                    xw.WriteEndElement();
                    xw.WriteStartElement("description");
                    xw.WriteCData(desc);
                    xw.WriteEndElement();
                    xw.WriteStartElement("styleUrl");
                    xw.WriteString("#downArrowIconStart");
                    xw.WriteEndElement();
                    xw.WriteStartElement("Point");
                    xw.WriteStartElement("coordinates");
                    xw.WriteString(session.StartLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + session.StartLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0");
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                if (session.EndLat.HasValue && session.EndLon.HasValue && !double.IsNaN(session.EndLat.Value) && !double.IsNaN(session.EndLon.Value) && session.Distance.HasValue && session.Duration.HasValue)
                {
                    xw.WriteStartElement("Placemark");
                    xw.WriteStartElement("name");
                    //xw.WriteString(string.Format(AppResources.NameKml, session.Paths.description, Utils.FromNOW(session.dayOfSession)));
                    xw.WriteString(session.Distance.Value.ToString("0", CultureInfo.InvariantCulture.NumberFormat) + "|" + TimeSpan.FromTicks(session.Duration.Value).ToString("hh':'mm':'ss", CultureInfo.InvariantCulture));
                    xw.WriteEndElement();
                    xw.WriteStartElement("description");
                    xw.WriteCData(desc);
                    xw.WriteEndElement();
                    xw.WriteStartElement("styleUrl");
                    xw.WriteString("#downArrowIconEnd");
                    xw.WriteEndElement();
                    xw.WriteStartElement("Point");
                    xw.WriteStartElement("coordinates");
                    xw.WriteString(session.EndLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + session.EndLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) + ",0");
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();

                xw.WriteStartElement("Placemark");

                xw.WriteStartElement("name");
                xw.WriteString(string.Format(resourceLoader.GetString("NameKml"), path.Description, session.DayOfSession));
                xw.WriteEndElement(); // name

                xw.WriteStartElement("styleUrl");
                xw.WriteString("#yellowLineGreenPoly");
                xw.WriteEndElement(); //styleUrl


                xw.WriteStartElement("LineString");

                xw.WriteStartElement("extrude");
                xw.WriteString("1");
                xw.WriteEndElement(); //extrude

                xw.WriteStartElement("tessellate");
                xw.WriteString("1");                    // 1 -> sigue el relieve, 0-> linea por encima
                xw.WriteEndElement(); // tessellate

                //<altitudeMode>absolute</altitudeMode> poniendo un valor de altura
                // <altitudeMode>relativeToGround</altitudeMode>

                /*xw.WriteStartElement("altitudeMode");
                xw.WriteString("absolute");
                xw.WriteEndElement();*/

                xw.WriteStartElement("coordinates");
                int N = 0;
                var sessions = DataBaseManager.instance.GetMesures(idSession);

                foreach (Mesures tmp in sessions)
                {
                    if (tmp.Action != null && "RESUME".Equals(tmp.Action))
                    {
                        xw.WriteEndElement(); // coordinates
                        xw.WriteEndElement(); // LineString
                        xw.WriteEndElement(); // Placemark

                        xw.WriteStartElement("Placemark");
                        xw.WriteStartElement("name");
                        xw.WriteString(string.Format(resourceLoader.GetString("DetailKml"), path.Description, session.DayOfSession));
                        xw.WriteEndElement(); // name
                        xw.WriteStartElement("styleUrl");
                        xw.WriteString("#yellowLineGreenPoly");
                        xw.WriteEndElement(); //styleUrl
                        xw.WriteStartElement("LineString");
                        xw.WriteStartElement("extrude");
                        xw.WriteString("1");
                        xw.WriteEndElement(); //extrude
                        xw.WriteStartElement("tessellate");
                        xw.WriteString("1");                    // 1 -> sigue el relieve, 0-> linea por encima
                        xw.WriteEndElement(); // tessellate
                        xw.WriteStartElement("coordinates");

                        N = 0;
                    }

                    if (N > 0)
                        xw.WriteString(" ");
                    xw.WriteString(tmp.Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + tmp.Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + (tmp.Altitude.HasValue ? tmp.Altitude.Value.ToString(CultureInfo.InvariantCulture.NumberFormat) : "0"));
                    N++;
                }
                xw.WriteEndElement(); // coordinates
                xw.WriteEndElement(); // LineString
                xw.WriteEndElement(); // Placemark


                xw.WriteEndElement(); // Document
                xw.WriteEndElement(); //kml
                xw.WriteEndDocument();
                xw.Flush();

            }

            return file;
        }

        static public async Task<StorageFile> exportGpx(int idSession, string name, string unit, ResourceLoader resourceLoader)
        {
            var cult = CultureInfo.CurrentUICulture;
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);

            using (var isoFileStream = await file.OpenStreamForWriteAsync())
            {
                XmlWriterSettings wSettings = new XmlWriterSettings();
                wSettings.Indent = true;

                XmlWriter xw = XmlWriter.Create(isoFileStream, wSettings);// Write Declaration
                xw.WriteStartDocument();

                double minLatitude = Double.MaxValue;
                double maxLatitude = Double.MinValue;
                double minLongitude = Double.MaxValue;
                double maxLongitude = Double.MinValue;

                double north = 0;
                double west = 0;
                double south = 0;
                double east = 0;

                var sessions0 = DataBaseManager.instance.GetMesures(idSession);
                int previousIndex = -1;
                foreach (Mesures tmp in sessions0)
                {
                    minLatitude = Math.Min(minLatitude, tmp.Latitude);
                    maxLatitude = Math.Max(maxLatitude, tmp.Latitude);
                    minLongitude = Math.Min(minLongitude, tmp.Longitude);
                    maxLongitude = Math.Max(maxLongitude, tmp.Longitude);

                    if (previousIndex == -1)
                    {
                        north = south = tmp.Latitude;
                        west = east = tmp.Longitude;
                    }
                    else
                    {
                        if (north < tmp.Latitude) north = tmp.Latitude;
                        if (west > tmp.Longitude) west = tmp.Longitude;
                        if (south > tmp.Latitude) south = tmp.Latitude;
                        if (east < tmp.Longitude) east = tmp.Longitude;
                        previousIndex = 1;
                    }
                }


                xw.WriteStartElement("gpx");
                xw.WriteStartAttribute("version");
                xw.WriteString("1.0");
                xw.WriteEndAttribute();
                xw.WriteStartAttribute("creator");
                xw.WriteString("Keep The Rhythm");
                xw.WriteEndAttribute();

                var session = DataBaseManager.instance.GetSession(idSession);
                var path = DataBaseManager.instance.GetPath(session.IdPath);
                var sessions = DataBaseManager.instance.GetMesures(idSession);

                /*xw.WriteStartElement("time");
                xw.WriteString(session.dayOfSession);
                xw.WriteEndElement();*/

                DateTime start = DateTime.Parse(session.DayOfSession);
                long startId = start.Ticks;

                xw.WriteStartElement("bounds");

                xw.WriteStartAttribute("minlon");
                xw.WriteString(minLongitude.ToString());
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("minlat");
                xw.WriteString(minLatitude.ToString());
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("maxlat");
                xw.WriteString(maxLatitude.ToString());
                xw.WriteEndAttribute();

                xw.WriteStartAttribute("maxlon");
                xw.WriteString(maxLongitude.ToString());
                xw.WriteEndAttribute();

                xw.WriteEndElement();

                xw.WriteStartElement("wpt");
                xw.WriteStartAttribute("lat");
                xw.WriteString(((minLatitude + maxLatitude) / 2).ToString());
                xw.WriteEndAttribute();
                xw.WriteStartAttribute("lon");
                xw.WriteString(((minLongitude + maxLongitude) / 2).ToString());
                xw.WriteEndAttribute();

                xw.WriteStartElement("name");
                xw.WriteString(path.Description + " - " + session.DayOfSession);
                xw.WriteEndElement();


                /*xw.WriteStartElement("name");
                xw.WriteString(new DateTime(tmp.id).ToShortDateString());
                xw.WriteEndElement();*/

                /*xw.WriteStartElement("sym");
                xw.WriteString("Dot");
                xw.WriteEndElement();*/

                xw.WriteEndElement();

                xw.WriteStartElement("trk");
                xw.WriteStartElement("trkseg");
                int N = 0;
                foreach (Mesures tmp in sessions)
                {
                    /*
<?xml version="1.0" encoding="UTF-8"?>
<gpx version="1.0">
<name>Example gpx</name>
<wpt lat="46.57638889" lon="8.89263889">
<ele>2372</ele>
<name>LAGORETICO</name>
</wpt>
<trk><name>Example gpx</name><number>1</number><trkseg>
<trkpt lat="46.57608333" lon="8.89241667"><ele>2376</ele><time>2007-10-14T10:09:57Z</time></trkpt>
<trkpt lat="46.57619444" lon="8.89252778"><ele>2375</ele><time>2007-10-14T10:10:52Z</time></trkpt>
<trkpt lat="46.57641667" lon="8.89266667"><ele>2372</ele><time>2007-10-14T10:12:39Z</time></trkpt>
<trkpt lat="46.57650000" lon="8.89280556"><ele>2373</ele><time>2007-10-14T10:13:12Z</time></trkpt>
<trkpt lat="46.57638889" lon="8.89302778"><ele>2374</ele><time>2007-10-14T10:13:20Z</time></trkpt>
<trkpt lat="46.57652778" lon="8.89322222"><ele>2375</ele><time>2007-10-14T10:13:48Z</time></trkpt>
<trkpt lat="46.57661111" lon="8.89344444"><ele>2376</ele><time>2007-10-14T10:14:08Z</time></trkpt>
</trkseg></trk>
</gpx>                     
*/
                    xw.WriteStartElement("trkpt");

                    xw.WriteStartAttribute("lat");
                    xw.WriteString(tmp.Latitude.ToString());
                    xw.WriteEndAttribute();
                    xw.WriteStartAttribute("lon");
                    xw.WriteString(tmp.Longitude.ToString());
                    xw.WriteEndAttribute();

                    xw.WriteStartElement("ele");
                    xw.WriteString((++N).ToString());
                    xw.WriteEndElement();

                    xw.WriteStartElement("time");
                    xw.WriteString(new DateTime(tmp.Id + startId).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"));
                    xw.WriteEndElement();

                    /*xw.WriteStartElement("sym");
                    xw.WriteString("Dot");
                    xw.WriteEndElement();*/

                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Flush();

            }

            return file;
        }

        //public void exportInternal(Sessions sessionObj, XmlWriter xw)
        //{
        //    xw.WriteStartElement("session");

        //    xw.WriteStartAttribute("id");
        //    xw.WriteString(sessionObj.id.ToString());
        //    xw.WriteEndAttribute();

        //    if (sessionObj.distance.HasValue)
        //    {
        //        xw.WriteStartAttribute("distance");
        //        xw.WriteString(sessionObj.distance.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.objDistance.HasValue)
        //    {
        //        xw.WriteStartAttribute("objDistance");
        //        xw.WriteString(sessionObj.objDistance.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.objective.HasValue)
        //    {
        //        xw.WriteStartAttribute("objective");
        //        xw.WriteString(sessionObj.objective.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.objTime.HasValue)
        //    {
        //        xw.WriteStartAttribute("objTime");
        //        xw.WriteString(sessionObj.objTime.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    xw.WriteStartAttribute("dayOfSession");
        //    xw.WriteString(sessionObj.dayOfSession);
        //    xw.WriteEndAttribute();

        //    if (sessionObj.duration.HasValue)
        //    {
        //        xw.WriteStartAttribute("duration");
        //        xw.WriteString(sessionObj.duration.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.ascendent.HasValue)
        //    {
        //        xw.WriteStartAttribute("ascendent");
        //        xw.WriteString(sessionObj.ascendent.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.descendent.HasValue)
        //    {
        //        xw.WriteStartAttribute("descendent");
        //        xw.WriteString(sessionObj.descendent.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.startLat.HasValue)
        //    {
        //        xw.WriteStartAttribute("startLat");
        //        xw.WriteString(sessionObj.startLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.startLon.HasValue)
        //    {
        //        xw.WriteStartAttribute("startLon");
        //        xw.WriteString(sessionObj.startLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.endLat.HasValue)
        //    {
        //        xw.WriteStartAttribute("endLat");
        //        xw.WriteString(sessionObj.endLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.endLon.HasValue)
        //    {
        //        xw.WriteStartAttribute("endLon");
        //        xw.WriteString(sessionObj.endLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.minLat.HasValue)
        //    {
        //        xw.WriteStartAttribute("minLat");
        //        xw.WriteString(sessionObj.minLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.minLon.HasValue)
        //    {
        //        xw.WriteStartAttribute("minLon");
        //        xw.WriteString(sessionObj.minLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.maxLat.HasValue)
        //    {
        //        xw.WriteStartAttribute("maxLat");
        //        xw.WriteString(sessionObj.maxLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.maxLon.HasValue)
        //    {
        //        xw.WriteStartAttribute("maxLon");
        //        xw.WriteString(sessionObj.maxLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.minAltitude.HasValue)
        //    {
        //        xw.WriteStartAttribute("minAltitude");
        //        xw.WriteString(sessionObj.minAltitude.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.maxAltitude.HasValue)
        //    {
        //        xw.WriteStartAttribute("maxAltitude");
        //        xw.WriteString(sessionObj.maxAltitude.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.minSpeed.HasValue)
        //    {
        //        xw.WriteStartAttribute("minSpeed");
        //        xw.WriteString(sessionObj.minSpeed.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.maxSpeed.HasValue)
        //    {
        //        xw.WriteStartAttribute("maxSpeed");
        //        xw.WriteString(sessionObj.maxSpeed.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.avgAltitude.HasValue)
        //    {
        //        xw.WriteStartAttribute("avgAltitude");
        //        xw.WriteString(sessionObj.avgAltitude.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.avgPace.HasValue)
        //    {
        //        xw.WriteStartAttribute("avgPace");
        //        xw.WriteString(sessionObj.avgPace.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.avgSpeed.HasValue)
        //    {
        //        xw.WriteStartAttribute("avgSpeed");
        //        xw.WriteString(sessionObj.avgSpeed.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    if (sessionObj.centerLat.HasValue)
        //    {
        //        xw.WriteStartAttribute("centerLat");
        //        xw.WriteString(sessionObj.centerLat.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }
        //    if (sessionObj.centerLon.HasValue)
        //    {
        //        xw.WriteStartAttribute("centerLon");
        //        xw.WriteString(sessionObj.centerLon.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //    }

        //    xw.WriteStartElement("comment");
        //    xw.WriteString(sessionObj.comment);
        //    xw.WriteEndElement();

        //    xw.WriteStartElement("points");
        //    foreach (PointInterest tmp in getPoints(sessionObj.id))
        //    {
        //        if (Constants.HIGHER.Equals(tmp.message) || Constants.LOWER.Equals(tmp.message) || tmp.message.StartsWith(Constants.EXTRA_POINTS))
        //        {
        //            continue;
        //        }

        //        xw.WriteStartElement("q");

        //        xw.WriteStartAttribute("i");
        //        xw.WriteString(tmp.id.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //        xw.WriteStartAttribute("o");
        //        xw.WriteString(tmp.longitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //        xw.WriteStartAttribute("a");
        //        xw.WriteString(tmp.latitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //        xw.WriteStartAttribute("d");
        //        xw.WriteString(tmp.distance.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();

        //        if (tmp.rhythm.HasValue)
        //        {
        //            xw.WriteStartAttribute("r");
        //            xw.WriteString(tmp.rhythm.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //            xw.WriteEndAttribute();
        //        }

        //        xw.WriteStartAttribute("t");
        //        xw.WriteString(tmp.time.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();

        //        xw.WriteStartAttribute("x");
        //        xw.WriteString(tmp.message);
        //        xw.WriteEndAttribute();

        //        xw.WriteEndElement();
        //    }
        //    xw.WriteEndElement();


        //    xw.WriteStartElement("coordinates");

        //    foreach (Mesures tmp in getMesures(sessionObj.id))
        //    {
        //        xw.WriteStartElement("p");

        //        xw.WriteStartAttribute("i");
        //        xw.WriteString(tmp.id.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //        if (tmp.action != null)
        //        {
        //            xw.WriteStartAttribute("x");
        //            xw.WriteString(tmp.action);
        //            xw.WriteEndAttribute();
        //        }
        //        xw.WriteStartAttribute("o");
        //        xw.WriteString(tmp.longitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //        xw.WriteStartAttribute("a");
        //        xw.WriteString(tmp.latitude.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //        xw.WriteEndAttribute();
        //        if (tmp.altitude.HasValue)
        //        {
        //            xw.WriteStartAttribute("l");
        //            xw.WriteString(tmp.altitude.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //            xw.WriteEndAttribute();
        //        }
        //        if (tmp.speed.HasValue)
        //        {
        //            xw.WriteStartAttribute("s");
        //            xw.WriteString(tmp.speed.Value.ToString(CultureInfo.InvariantCulture.NumberFormat));
        //            xw.WriteEndAttribute();
        //        }
        //        xw.WriteEndElement();
        //    }

        //    xw.WriteEndElement();
        //}

        //public Sessions importInternal(XmlReader reader)
        //{
        //    string text = "";

        //    Sessions sessionObj = null;

        //    do
        //    {
        //        switch (reader.NodeType)
        //        {
        //            case XmlNodeType.Element:
        //                text = "";
        //                if (string.Equals(reader.Name, "session", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    sessionObj = new Sessions();

        //                    if (reader.MoveToFirstAttribute())
        //                    {
        //                        do
        //                        {
        //                            if (string.Equals(reader.Name, "id", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                //sessionObj.id = reader.Value;
        //                            }
        //                            else if (string.Equals(reader.Name, "ascendent", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.ascendent = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "dayOfSession", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.dayOfSession = reader.Value;
        //                            }
        //                            else if (string.Equals(reader.Name, "descendent", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.descendent = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "distance", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.distance = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "duration", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.duration = long.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "objDistance", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.objDistance = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "objective", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.objective = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "objTime", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.objTime = long.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "startLat", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.startLat = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "startLon", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.startLon = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "endLat", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.endLat = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "endLon", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.endLon = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "minAltitude", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.minAltitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "maxAltitude", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.maxAltitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "minSpeed", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.minSpeed = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "maxSpeed", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.maxSpeed = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "avgSpeed", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.avgSpeed = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "avgPace", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.avgPace = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "avgAltitude", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.avgAltitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "minLat", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.minLat = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "maxLat", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.maxLat = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "minLon", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.minLon = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "maxLon", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.maxLon = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "centerLat", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.centerLat = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "centerLon", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                sessionObj.centerLon = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                        } while (reader.MoveToNextAttribute());
        //                    }
        //                }
        //                else if (string.Equals(reader.Name, "p", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    Mesures p = new Mesures();

        //                    if (reader.MoveToFirstAttribute())
        //                    {
        //                        do
        //                        {
        //                            if (string.Equals(reader.Name, "i", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.id = long.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "o", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.longitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "a", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.latitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "l", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.altitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "s", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.speed = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "x", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.action = reader.Value;
        //                            }
        //                        } while (reader.MoveToNextAttribute());
        //                    }

        //                    sessionObj.Mesures.Add(p);
        //                }
        //                else if (string.Equals(reader.Name, "q", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    PointInterest p = new PointInterest();

        //                    if (reader.MoveToFirstAttribute())
        //                    {
        //                        do
        //                        {
        //                            if (string.Equals(reader.Name, "i", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                //p.id = int.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "o", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.longitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "a", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.latitude = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "d", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.distance = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "r", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.rhythm = double.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "t", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.time = long.Parse(reader.Value, CultureInfo.InvariantCulture.NumberFormat);
        //                            }
        //                            else if (string.Equals(reader.Name, "x", StringComparison.OrdinalIgnoreCase))
        //                            {
        //                                p.message = reader.Value;
        //                            }

        //                        } while (reader.MoveToNextAttribute());
        //                    }

        //                    sessionObj.PointInterests.Add(p);
        //                }
        //                break;
        //            case XmlNodeType.Text:
        //                //writer.WriteString(reader.Value);
        //                text += reader.Value;
        //                break;
        //            case XmlNodeType.XmlDeclaration:
        //            case XmlNodeType.ProcessingInstruction:
        //                //writer.WriteProcessingInstruction(reader.Name, reader.Value);
        //                break;
        //            case XmlNodeType.Comment:
        //                //writer.WriteComment(reader.Value);
        //                break;
        //            case XmlNodeType.EndElement:
        //                if (string.Equals(reader.Name, "comment", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    sessionObj.comment = text;
        //                }
        //                else if (string.Equals(reader.Name, "session", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    return sessionObj;
        //                }
        //                break;
        //        }
        //    }
        //    while (reader.Read());

        //    return sessionObj;
        //}

        static public async Task<string> ImportKmlAsync(IStorageItem file, bool modeTest, Paths path, Sessions session, IProgress<int> progress)
        {
            int n = 0;
            string description = null;
            int NN = 0;
            string mode = path.Type;

            progress.Report(n++);
            using (var read = await (file as IStorageFile).OpenReadAsync())
            {
                progress.Report(n++);
                using (XmlReader reader = XmlReader.Create(read.AsStreamForRead())) //isoFileStream)) //new StringReader(xmlString)))
                {
                    bool line = false, coord = false;

                    char[] sep = { ',' };
                    char[] sep2 = { '|' };

                    double centerLat = 0;
                    double centerLon = 0;

                    double maxAltitude = Double.MinValue;
                    double minAltitude = Double.MaxValue;

                    double maxLat = Double.MinValue;
                    double maxLon = Double.MinValue;
                    double minLat = Double.MaxValue;
                    double minLon = Double.MaxValue;

                    long Na = 0; //, Ns = 0;
                    double avgAltitude = 0;

                    double ALTITUDE_LIMIT;
                    double ALTITUDE_LIMIT_MIN;

                    double currentDistance = 0;
                    BasicGeoposition? previousPoint = null;

                    ALTITUDE_LIMIT = nowhereman.Properties.getDoubleProperty("maxAltitude" + mode, 8848);
                    ALTITUDE_LIMIT_MIN = nowhereman.Properties.getDoubleProperty("minAltitude" + mode, -10);

                    string text = null;
                    string textTmp = "";

                    string tmpdescription = null;
                    string tmpstyleurl = null;
                    string tmpcoordinates = null;
                    string tmpname = null;
                    progress.Report(n++);
                    while (reader.Read())
                    {
                        progress.Report(n++);

                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                System.Diagnostics.Debug.WriteLine(">> " + reader.Name + " " + line.ToString() + " " + coord.ToString());
                                if (String.Equals(reader.Name, "Document", StringComparison.OrdinalIgnoreCase))
                                {

                                }
                                else if (String.Equals(reader.Name, "Description", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (description == null)
                                    {
                                        text = "";
                                    }
                                }
                                else if (String.Equals(reader.Name, "LineString", StringComparison.OrdinalIgnoreCase))
                                {
                                    line = true;
                                }
                                else if (String.Equals(reader.Name, "coordinates", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (line)
                                        coord = true;
                                }
                                else if (String.Equals(reader.Name, "Placemark", StringComparison.OrdinalIgnoreCase))
                                {
                                    tmpdescription = null;
                                    tmpstyleurl = null;
                                    tmpcoordinates = null;
                                    tmpname = null;
                                    textTmp = "";
                                }
                                break;
                            case XmlNodeType.Text:
                                {
                                    if (coord)
                                    {
                                        double[] vv = new double[3];
                                        string s = reader.Value;
                                        int i = 0, j = 0, N = s.Length;

                                        while (i <= N)
                                        {
                                            progress.Report(n++);

                                            char c = i == N ? ' ' : s[i];
                                            if (Char.IsWhiteSpace(c))
                                            {
                                                if (j < i)
                                                {
                                                    string _v = "";
                                                    try
                                                    {
                                                        _v = s.Substring(j, i - j);

                                                        string[] v = _v.Split(sep);

                                                        double longitude = Double.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat);
                                                        if (v.Length >= 1)
                                                        {
                                                            double latitude = Double.Parse(v[1], CultureInfo.InvariantCulture.NumberFormat);
                                                            double? altitude = null;

                                                            if (v.Length >= 2)
                                                            {
                                                                altitude = Double.Parse(v[2], CultureInfo.InvariantCulture.NumberFormat);
                                                            }

                                                            NN++;
                                                            Mesures mesure = null;
                                                            if (!modeTest)
                                                            {
                                                                mesure = new Mesures();
                                                                mesure.IdSession = session.Id;
                                                                mesure.Latitude = latitude;
                                                                mesure.Longitude = longitude;
                                                                mesure.Altitude = altitude;
                                                                mesure.Speed = null;
                                                                mesure.Id = NN;
                                                            }
                                                            if (session.StartLat == null)
                                                            {
                                                                session.StartLat = latitude;
                                                                session.StartLon = longitude;
                                                            }
                                                            session.EndLat = latitude;
                                                            session.EndLon = longitude;


                                                            var L = new BasicGeoposition() { Latitude = latitude, Longitude = longitude, Altitude = altitude.HasValue ? altitude.Value : 0.0 };
                                                            if (previousPoint.HasValue)
                                                            {
                                                                double dist = previousPoint.Value.GetDistanceTo(L);
                                                                currentDistance += dist;
                                                            }
                                                            previousPoint = L;

                                                            if (!modeTest)
                                                            {
                                                                DataBaseManager.instance.InsertMesure(mesure);

                                                            }
                                                            if (altitude.HasValue)
                                                            {
                                                                if (altitude >= ALTITUDE_LIMIT_MIN && altitude <= ALTITUDE_LIMIT)
                                                                {
                                                                    if (maxAltitude < altitude)
                                                                    {
                                                                        maxAltitude = Math.Max(maxAltitude, altitude.Value);

                                                                    }
                                                                    if (minAltitude > altitude)
                                                                    {
                                                                        minAltitude = Math.Min(minAltitude, altitude.Value);

                                                                    }
                                                                    if (altitude.Value != 0) avgAltitude += 1 / altitude.Value;
                                                                    Na++;
                                                                }
                                                            }

                                                            centerLat += latitude;
                                                            centerLon += longitude;

                                                            maxLat = Math.Max(maxLat, latitude);
                                                            minLat = Math.Min(minLat, latitude);

                                                            maxLon = Math.Max(maxLon, longitude);
                                                            minLon = Math.Min(minLon, longitude);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        nowhereman.LittleWatson.instance.Error("detail3 import process", ex);
                                                        Console.WriteLine("error coordinates " + ex.Message + " " + _v);
                                                    }
                                                    i++;
                                                    while (i < N && Char.IsWhiteSpace(s[i]))
                                                    {
                                                        i++;
                                                    }
                                                }
                                                else
                                                {
                                                    i++;
                                                }
                                                j = i;
                                            }
                                            else
                                            {
                                                i++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (text != null)
                                        {
                                            text += reader.Value;
                                        }
                                        else
                                        {
                                            textTmp += reader.Value;
                                        }
                                    }
                                }
                                break;
                            case XmlNodeType.CDATA:
                                if (text != null)
                                {
                                    text = reader.Value;
                                }
                                else
                                {
                                    textTmp = reader.Value;
                                }
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                //writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                //writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:
                                System.Diagnostics.Debug.WriteLine("<< " + reader.Name + " " + line.ToString() + " " + coord.ToString());
                                if (String.Equals(reader.Name, "Document", StringComparison.OrdinalIgnoreCase))
                                {

                                    if (NN > 0)
                                    {
                                        session.CenterLat = centerLat / NN;
                                        session.CenterLon = centerLon / NN;
                                        session.MaxLat = maxLat;
                                        session.MaxLon = maxLon;
                                        session.MinLat = minLat;
                                        session.MinLon = minLon;

                                        session.Distance = currentDistance;
                                    }

                                    session.AvgSpeed = 0;

                                    session.MinSpeed = 0;
                                    session.MaxSpeed = 0;


                                    if (avgAltitude != 0 && Na > 0)
                                    {
                                        session.AvgAltitude = Na / avgAltitude;
                                        session.MaxAltitude = maxAltitude;
                                        session.MinAltitude = minAltitude;
                                    }
                                    else
                                    {
                                        session.AvgAltitude = 0;
                                        session.MaxAltitude = 0;
                                        session.MinAltitude = 0;
                                    }


                                }
                                else if (String.Equals(reader.Name, "styleUrl", StringComparison.OrdinalIgnoreCase))
                                {
                                    tmpstyleurl = textTmp;
                                    textTmp = "";
                                }
                                else if (String.Equals(reader.Name, "Name", StringComparison.OrdinalIgnoreCase))
                                {
                                    tmpname = textTmp;
                                    textTmp = "";
                                }
                                else if (String.Equals(reader.Name, "Description", StringComparison.OrdinalIgnoreCase))
                                {
                                    tmpdescription = textTmp;
                                    if (text != null)
                                    {
                                        description = text;
                                        text = null;
                                    }
                                    textTmp = "";
                                }
                                else if (String.Equals(reader.Name, "LineString", StringComparison.OrdinalIgnoreCase))
                                {
                                    line = false;
                                    coord = false;
                                }
                                else if (String.Equals(reader.Name, "coordinates", StringComparison.OrdinalIgnoreCase))
                                {
                                    tmpcoordinates = textTmp;
                                    coord = false;
                                    textTmp = "";
                                }
                                else if (String.Equals(reader.Name, "Placemark", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (String.Equals(tmpstyleurl, "#downArrowIconStart", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            string[] v = tmpcoordinates.Split(sep);

                                            double longitude = Double.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat);
                                            if (v.Length >= 1)
                                            {
                                                double latitude = Double.Parse(v[1], CultureInfo.InvariantCulture.NumberFormat);

                                                session.StartLon = longitude;
                                                session.StartLat = latitude;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            nowhereman.LittleWatson.instance.Error("detail2 import process", ex);
                                            System.Diagnostics.Debug.WriteLine("error downArrowIconStart with " + tmpdescription + " " + ex.Message);
                                            throw ex;
                                        }

                                    }
                                    else if (String.Equals(tmpstyleurl, "#downArrowIconEnd", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            string[] v = tmpcoordinates.Split(sep);

                                            double longitude = Double.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat);
                                            if (v.Length >= 1)
                                            {
                                                double latitude = Double.Parse(v[1], CultureInfo.InvariantCulture.NumberFormat);

                                                v = tmpname.Split(sep2);
                                                double dist = Double.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat);
                                                if (v.Length >= 1)
                                                {
                                                    long tim = TimeSpan.ParseExact(v[1], "hh':'mm':'ss", CultureInfo.InvariantCulture).Ticks;

                                                    session.EndLon = longitude;
                                                    session.EndLat = latitude;
                                                    session.Distance = dist;
                                                    session.Duration = tim;
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            nowhereman.LittleWatson.instance.Error("detail1 import process", ex);
                                            System.Diagnostics.Debug.WriteLine("error downArrowIconEnd with " + tmpdescription + " " + ex.Message);
                                            throw ex;
                                        }

                                    }
                                    else if (String.Equals(tmpstyleurl, "#downArrowIcon", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            string[] v = tmpcoordinates.Split(sep);

                                            double longitude = Double.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat);
                                            if (v.Length >= 1)
                                            {
                                                double latitude = Double.Parse(v[1], CultureInfo.InvariantCulture.NumberFormat);

                                                v = tmpname.Split(sep2);
                                                double dist = Double.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat);
                                                if (v.Length >= 1)
                                                {
                                                    long tim = TimeSpan.ParseExact(v[1], "hh':'mm':'ss", CultureInfo.InvariantCulture).Ticks;

                                                    if (tmpdescription.IndexOf('|') < 0)
                                                    {
                                                        /*
                                                         * <description><![CDATA[<div itemprop="track" itemscope itemtype="http://schema.org/MusicRecording"><span itemprop="name">Just Give Me A Reason</span><span itemprop="byArtist">P!nk Feat. Nate Ruess</span><meta content="The Truth About Love" itemprop="inAlbum" /></div>]]></description>
                                                         */
                                                        string c0 = "";
                                                        string c1 = "";
                                                        string c2 = "";

                                                        int i = tmpdescription.IndexOf("<span itemprop=\"name\">");
                                                        int j = 0;
                                                        if (i >= 0)
                                                        {
                                                            j = tmpdescription.IndexOf("</span>", i);
                                                            if (j > 0)
                                                            {
                                                                int l = "<span itemprop=\"name\">".Length;
                                                                c2 = tmpdescription.Substring(i + l, j - i - l);
                                                            }
                                                        }
                                                        i = tmpdescription.IndexOf("<span itemprop=\"byArtist\">", j);
                                                        if (i >= 0)
                                                        {
                                                            j = tmpdescription.IndexOf("</span>", i);
                                                            if (j > 0)
                                                            {
                                                                int l = "<span itemprop=\"byArtist\">".Length;
                                                                c0 = tmpdescription.Substring(i + l, j - i - l);
                                                            }
                                                        }
                                                        i = tmpdescription.IndexOf("<meta content=\"", j);
                                                        if (i >= 0)
                                                        {
                                                            j = tmpdescription.IndexOf("\" itemprop=\"inAlbum\"", i);
                                                            if (j > 0)
                                                            {
                                                                int l = "<meta content=\"".Length;
                                                                c1 = tmpdescription.Substring(i + l, j - i - l);
                                                            }
                                                        }

                                                        tmpdescription = c0 + "|" + c1 + "|" + c2;
                                                    }

                                                    if (!modeTest)
                                                    {
                                                        PointInterest pi = new PointInterest();
                                                        pi.IdSession = session.Id;
                                                        pi.Message = tmpdescription;
                                                        pi.Longitude = longitude;
                                                        pi.Latitude = latitude;
                                                        pi.Time = tim;
                                                        pi.Distance = dist;
                                                        pi.Type = mode;

                                                        DataBaseManager.instance.InsertPoint(pi);

                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            nowhereman.LittleWatson.instance.Error("detail import process", ex);
                                            System.Diagnostics.Debug.WriteLine("error downArrowIcon with " + tmpdescription + " " + ex.Message);
                                            throw ex;
                                        }
                                    }


                                    tmpdescription = null;
                                    tmpstyleurl = null;
                                    tmpcoordinates = null;
                                    tmpname = null;
                                }
                                break;
                        }
                    }

                }

                progress.Report(n++);
                if (!modeTest && description != null)
                {
                    DataBaseManager.instance.UpdateSession(session);
                }
                progress.Report(n++);
                return description;
            }
        }
    }
}
