using System;
using Windows.Devices.Geolocation;

namespace UniversalKeepTheRhythm.Services
{
    public static class DistanceCalculator
    {
        private const double ApproxEarthRadius = 6371d;

        private const double PI180 = (Math.PI / 180d);

        /// <summary>
        /// Calculates the distance between two geodetic coordinates using the Haversine formula.
        /// </summary>
        /// <param name="coordinate1">The first coordinate.</param>
        /// <param name="coordinate2">The second coordinate.</param>
        /// <returns>The distance between the coordinates in kilometers.</returns>
        public static double Haversine(Geocoordinate coordinate1, Geocoordinate coordinate2)
        {
            var p1 = coordinate1.Point.Position;
            var p2 = coordinate2.Point.Position;

            double latDelta = (p1.Latitude - p2.Latitude) * PI180;
            double lonDelta = (p1.Longitude - p2.Longitude) * PI180;

            double a = Math.Sin(latDelta / 2.0) * Math.Sin(latDelta / 2.0) +
                       Math.Cos(p1.Latitude * PI180) * Math.Cos(p2.Latitude * PI180) *
                       Math.Sin(lonDelta / 2.0) * Math.Sin(lonDelta / 2.0);
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
            return ApproxEarthRadius * c;
        }

        /// <summary>
        /// Calculates the distance between two geodetic coordinates using the Spherical law of cosines.
        /// </summary>
        /// <param name="coordinate1">The first coordinate.</param>
        /// <param name="coordinate2">The second coordinate.</param>
        /// <returns>The distance between the coordinates in kilometers.</returns>
        public static double Spherical(Geocoordinate coordinate1, Geocoordinate coordinate2)
        {
            var p1 = coordinate1.Point.Position;
            var p2 = coordinate2.Point.Position;
            double d =
                Math.Acos(Math.Sin(p1.Latitude * PI180) * Math.Sin(p2.Latitude * PI180) +
                          Math.Cos(p1.Latitude * PI180) * Math.Cos(p2.Latitude * PI180) *
                          Math.Cos(p2.Longitude * PI180 - coordinate1.Point.Position.Longitude * PI180));
            return d * ApproxEarthRadius;
        }

        /// <summary>
        /// Calculates a new coordinate from a bearing and distance from a specified coordinate.
        /// </summary>
        /// <param name="start">The initial coordinate.</param>
        /// <param name="bearing">The bearing from the initial coordinate in degrees.</param>
        /// <param name="distance">The distance from the initial coordinate in kilometers.</param>
        /// <returns>A new geodetic coordinate representing the new point.</returns>
        //public static Geocoordinate CoordFromDistance(Geocoordinate start, double bearing, double distance)
        //{
        //    distance = distance / ApproxEarthRadius;
        //    bearing *= Math.PI / 180d;

        //    double latStart = start.Point.Position.Latitude.ToRadians();
        //    double lonStart = start.Point.Position.Longitude.ToRadians();

        //    var latEnd = Math.Asin(Math.Sin(latStart) * Math.Cos(distance) +
        //                          Math.Cos(latStart) * Math.Sin(distance) * Math.Cos(bearing));
        //    var lonEnd = lonStart + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance) * Math.Cos(latStart),
        //                                 Math.Cos(distance) - Math.Sin(latStart) * Math.Sin(latEnd));
        //    lonEnd = (lonEnd + 3 * Math.PI) % (2 * Math.PI) - Math.PI;  // normalise to -180...+180

        //    return new Geocoordinate() { Latitude = latEnd.ToDegrees(), Longitude = lonEnd.ToDegrees() };
        //}
    }

}


