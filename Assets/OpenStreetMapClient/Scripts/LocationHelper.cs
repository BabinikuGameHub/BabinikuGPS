using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSMClient
{
    public static class LocationHelper
    {
        public const double MetersPerLongitudeOnEquator = 111319.9;
        public const double MetersPerLatitude = 111132.9;

        /// <summary>Returns distance in meters between two locations</summary>
        public static double GetDistanceInMeters(Vector2d from, Vector2d to)
        {
            // Z -> north
            var dz = (from.y - to.y) * MetersPerLatitude;

            // X -> east
            var d = (from.x - to.x);
            if (d <= -180) d += 360;
            if (d > 180) d -= 360;
            var dx = d * Math.Cos(from.y * Mathf.Deg2Rad) * MetersPerLongitudeOnEquator;

            return Math.Sqrt(dz * dz + dx * dx);
        }

        /// <summary>Returns meter measured in lat/long</summary>
        public static Vector2d MeterToLongLat(Vector2d location)
        {
            // Z -> north
            var dz = 1d / MetersPerLatitude;

            // X -> east
            var dx = 1d / (Math.Cos(location.y * Mathf.Deg2Rad) * MetersPerLongitudeOnEquator);

            return new Vector2d(dx, dz);
        }

        public static Vector2d WorldToTilePos(double lon, double lat, int zoom)
        {
            var X = (lon + 180.0) / 360.0 * (1 << zoom);
            var Y = (1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom);

            return new Vector2d(X, Y);
        }

        public static Vector2d TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));

            var lon = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            var lat = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            if (lon > 180) lon -= 360;
            if (lon <= -180) lon += 360;

            return new Vector2d(lon, lat);
        }
    }

    public struct Vector2d
    {
        /// <summary>
        /// Longitude
        /// </summary>
        public double x;
        /// <summary>
        /// Latitude
        /// </summary>
        public double y;

        public Vector2d(double lon, double lat)
        {
            this.x = lon;
            this.y = lat;
        }

        public override string ToString()
        {
            return string.Format(" X={0:0.0000} Y={1:0.0000}", x, y);
        }

        public double magnitudeSqr => x * x + y * y;
        public double magnitude => Math.Sqrt(x * x + y * y);

        public static Vector2d Lerp(Vector2d from, Vector2d to, double t)
        {
            var t0 = 1 - t;
            return new Vector2d(from.x * t0 + to.x * t, from.y * t0 + to.y * t);
        }

        public static Vector2d operator +(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2d operator -(Vector2d v1, Vector2d v2)
        {
            return new Vector2d(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2d operator *(Vector2d v1, double k)
        {
            return new Vector2d(v1.x * k, v1.y * k);
        }

        public static Vector2d operator /(Vector2d v1, double k)
        {
            return new Vector2d(v1.x / k, v1.y / k);
        }

        public static double Dot(Vector2d v1, Vector2d v2)
        {
            return v1.x * v2.x + v1.y * v2.y;
        }
    }
}