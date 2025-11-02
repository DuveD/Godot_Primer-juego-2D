using Godot;
using System;

public class Randomizer
{
    /// <summary>
    /// Función que retorna un número aleatório entre minValue y maxValue.
    /// </summary>
    public static int GetRandomInt(int minValue, int maxValue)
    {
        Random random = new();
        return random.Next(minValue, maxValue);
    }

    /// <summary>
    /// Función que retorna un número aleatório entre 0.0 y 1.0.
    /// </summary>
    public static float GetRandomFloat()
    {
        return GD.Randf();
    }

    /// <summary>
    /// Función que retorna un número aleatório entre minValue y maxValue.
    /// </summary>
    public static double GetRandomDouble(double minValue, double maxValue)
    {
        return GD.RandRange(minValue, maxValue);
    }
}
