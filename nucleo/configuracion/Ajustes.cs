namespace Primerjuego2D.nucleo.ajustes;

using System;
using Godot;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.Logger;

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

    /// <summary>
    /// Nivel de log actual.
    /// </summary>
    public static NivelLog NivelLog { get; set; } = NivelLog.Trace;

    /// <summary>
    /// Indica si se debe escribir el log en un archivo.
    /// </summary>
    public static bool EscribirLogEnFichero { get; set; } = false;
}
