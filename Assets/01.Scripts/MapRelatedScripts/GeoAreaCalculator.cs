using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoAreaCalculator 
{
    private const double EarthRadius = 6371000;

    static double DegToRad(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    static double RadToDegree(double rad)
    {
        return rad * 180.0 / Math.PI; 
    }

    static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = DegToRad(lat2 - lat1);
        double dLon = DegToRad(lon2 - lon1);
        double a = Math.Sin((dLat / 2)) * Math.Sin((dLat / 2)) +
                   Math.Cos(DegToRad(lat1)) * Math.Cos(DegToRad(lat2)) *
                   Math.Sin((dLon / 2)) * Math.Sin((dLon / 2));
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt((1 - a)));
        return EarthRadius * c;
    }

    public static double SphericalTriangleArea(double a, double b, double c)
    {
        // Convert sides from meters to radians
        double aRad = a / EarthRadius;
        double bRad = b / EarthRadius;
        double cRad = c / EarthRadius;

        // Calculate the angles using the spherical law of cosines
        double test1 = Math.Cos(aRad);
        double test2 = Math.Cos(bRad);
        double test3 = Math.Cos(cRad);

        double alpha = Math.Acos(((Math.Cos(aRad) - (Math.Cos(bRad) * Math.Cos(cRad)))) / (Math.Sin(bRad) * Math.Sin(cRad)));
        double beta = Math.Acos(((Math.Cos(bRad) - (Math.Cos(aRad) * Math.Cos(cRad)))) / (Math.Sin(aRad) * Math.Sin(cRad)));
        double gamma = Math.Acos(((Math.Cos(cRad) - (Math.Cos(aRad) * Math.Cos(bRad)))) / (Math.Sin(aRad) * Math.Sin(bRad)));

        // Calculate the spherical excess
        double sphericalExcess = alpha + beta + gamma - Math.PI;

        double area1 = sphericalExcess * (EarthRadius * EarthRadius); 

        double alphaDegree = RadToDegree(alpha);
        double betaDegree = RadToDegree(beta);
        double gammaDegree = RadToDegree(gamma);

        double sphericalExcessDegree = alphaDegree + betaDegree + gammaDegree - 180;

        double area2 = sphericalExcessDegree * (EarthRadius * EarthRadius);

        // Calculate the area of the spherical triangle
        return area1;
    }

    public static double FlatTriangleArea(double a, double b, double c)
    {
        double s = (a + b + c) / 2;

        // Calculate the area of the spherical triangle
        return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
    }

    public static double CalculateArea(double lat1, double lon1, double lat2, double lon2, double lat3, double lon3)
    {
        double a = Haversine(lat1, lon1, lat2, lon2);
        double b = Haversine(lat2, lon2, lat3, lon3);
        double c = Haversine(lat3, lon3, lat1, lon1);

        //double E = SphericalExcess(a, b, c);
        //double radiusEarth = 6371000; // Earth's radius in meters
        //return E * radiusEarth * radiusEarth;

        //double sphericalArea = SphericalTriangleArea(a, b, c);
        double flatArea = FlatTriangleArea(a, b, c);

        //Debug.Log($"Calculated Spherical Area is {sphericalArea}");
        //Debug.Log($"Calculated Flat Area is {flatArea}");

        return flatArea;
    }
}
