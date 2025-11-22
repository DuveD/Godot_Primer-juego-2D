namespace Primerjuego2D.nucleo.utilidades;

using System;
using Godot;

public static class Randomizador
{
    // Instancia global compartida (para uso rápido y general)
    private static readonly Random GlobalRandom = new();

    // ---- MÉTODOS GENERALES ----

    /// <summary>
    /// Devuelve un número entero aleatorio entre minValue (incluido) y maxValue (excluido).
    /// Usa una instancia global compartida.
    /// </summary>
    public static int GetRandomInt(int minValue, int maxValue)
    {
        return GlobalRandom.Next(minValue, maxValue);
    }

    /// <summary>
    /// Devuelve un número aleatorio entre minValue y maxValue como double.
    /// Usa la instancia global.
    /// </summary>
    public static double GetRandomDouble(double minValue, double maxValue)
    {
        return GlobalRandom.NextDouble() * (maxValue - minValue) + minValue;
    }

    /// <summary>
    /// Devuelve un número aleatorio entre 0.0 y 1.0.
    /// </summary>
    public static float GetRandomFloat()
    {
        return (float)GlobalRandom.NextDouble();
    }

    // ---- MÉTODOS GODOT ----

    /// <summary>
    /// Devuelve un número entero aleatorio entre minValue y maxValue usando el RNG de Godot.
    /// </summary>
    public static int GetRandomIntGodot(int minValue, int maxValue)
    {
        return (int)GetRandomDoubleGodot(minValue, maxValue);
    }

    /// <summary>
    /// Devuelve un número flotante aleatorio entre 0.0 y 1.0 usando el RNG de Godot.
    /// </summary>
    public static float GetRandomFloatGodot()
    {
        return GD.Randf();
    }

    /// <summary>
    /// Devuelve un número aleatorio entre minValue y maxValue usando el RNG de Godot.
    /// </summary>
    public static double GetRandomDoubleGodot(double minValue, double maxValue)
    {
        return GD.RandRange(minValue, maxValue);
    }
}
