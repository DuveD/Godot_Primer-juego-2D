namespace Primerjuego2D.nucleo.utilidades;

using System;

public static class UtilidadesMatematicas
{
    /// <summary>
    /// FUnción que convierte radianes a grados.
    /// </summary>
    public static double RadiansToDegrees(double radians)
    {
        return (180 / Math.PI) * radians;
    }

    /// <summary>
    /// Función que convierte grados a radianes.
    /// </summary>
    public static double DegreesToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }
}