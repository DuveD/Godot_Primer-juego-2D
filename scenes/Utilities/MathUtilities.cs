using System;

public class MathUtilities
{
    public static double RadiansToDegrees(double radians)
    {
        return (180 / Math.PI) * radians;
    }
    
    public static double DegreesToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }
}