namespace Primerjuego2D.nucleo.ajustes;

using System;
using Godot;
using Primerjuego2D.nucleo.localizacion;

public static class Ajustes
{
    /// <summary>
    /// Idioma actual de la aplicación.
    /// </summary>
    public static Idioma IdiomaActual { get; set; } = Idioma.Castellano;

    /// <summary>
    /// Indica si el juego está pausado.
    /// </summary>
    public static bool JuegoPausado { get; set; } = false;
}
