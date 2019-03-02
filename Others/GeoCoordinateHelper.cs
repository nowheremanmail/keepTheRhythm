using System;
using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.Others
{
    static public class GeoCoordinateHelper
    {
        static public BasicGeoposition NoAltitude(this BasicGeoposition from)
        {
            return new BasicGeoposition() { Longitude = from.Longitude, Latitude = from.Latitude};
        }
        static public double GetDistanceTo(this BasicGeoposition from, BasicGeoposition to)
        {
                double latitude = from.Latitude * 0.0174532925199433;
                double longitude = from.Longitude * 0.0174532925199433;
                double num = to.Latitude * 0.0174532925199433;
                double longitude1 = to.Longitude * 0.0174532925199433;
                double num1 = longitude1 - longitude;
                double num2 = num - latitude;
                double num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
                double num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
                double num5 = 6376500 * num4;
                return num5;
        }

        static public double GetDistanceTo(this Geocoordinate orig, Geocoordinate other)
        {
            return GetDistanceTo(orig, other.Point.Position.Latitude, other.Point.Position.Longitude);
        }
        static public double GetDistanceTo(this Geocoordinate orig, double Lat, double Lon)
        {

            double latitude = orig.Point.Position.Latitude * 0.0174532925199433;
            double longitude = orig.Point.Position.Longitude * 0.0174532925199433;
            double num = Lat * 0.0174532925199433;
            double longitude1 = Lon * 0.0174532925199433;
            double num1 = longitude1 - longitude;
            double num2 = num - latitude;
            double num3 = Math.Pow(Math.Sin(num2 / 2), 2) + Math.Cos(latitude) * Math.Cos(num) * Math.Pow(Math.Sin(num1 / 2), 2);
            double num4 = 2 * Math.Atan2(Math.Sqrt(num3), Math.Sqrt(1 - num3));
            double num5 = 6376500 * num4;
            return num5;

        }
    }
}

