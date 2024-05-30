using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoAreaCalculator 
{
    static double DegToRad(double degrees)
    {
        return degrees * Mathf.PI / 180.0;
    }

    static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = DegToRad(lat2 - lat1);
        double dLon = DegToRad(lon2 - lon1);
        double a = Mathf.Sin((float)(dLat / 2)) * Mathf.Sin((float)(dLat / 2)) +
                   Mathf.Cos((float)DegToRad(lat1)) * Mathf.Cos((float)DegToRad(lat2)) *
                   Mathf.Sin((float)(dLon / 2)) * Mathf.Sin((float)(dLon / 2));
        double c = 2 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1 - a)));
        double radiusEarth = 6371000; // Earth's radius in meters
        return radiusEarth * c;
    }

    static double SphericalExcess(double a, double b, double c)
    {
        double s = (a + b + c) / 2;
        double tanE = Mathf.Tan((float)(s / 2)) * Mathf.Tan((float)((s - a) / 2)) * Mathf.Tan((float)((s - b) / 2)) * Mathf.Tan((float)((s - c) / 2));
        return 4 * Mathf.Atan(Mathf.Sqrt(Mathf.Abs((float)tanE)));
    }

    public static double CalculateArea(double lat1, double lon1, double lat2, double lon2, double lat3, double lon3)
    {
        double a = Haversine(lat1, lon1, lat2, lon2);
        double b = Haversine(lat2, lon2, lat3, lon3);
        double c = Haversine(lat3, lon3, lat1, lon1);

        double E = SphericalExcess(a, b, c);
        double radiusEarth = 6371000; // Earth's radius in meters
        return E * radiusEarth * radiusEarth;
    }
}
