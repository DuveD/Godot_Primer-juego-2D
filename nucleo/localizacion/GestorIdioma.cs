namespace Primerjuego2D.nucleo.localizacion;

using System.Collections.Generic;
using Godot;
using Microsoft.VisualBasic;

public class GestorIdioma
{
    public static readonly Dictionary<Idioma, string> IdiomasLocales = new()
    {
        { Idioma.Castellano, "es" },
        { Idioma.Ingles, "en" }
    };

    /// <summary>
    /// Establece el idioma de la aplicación.
    /// </summary>
    public static void SetIdioma(Idioma idioma)
    {
        string locale;

        switch (idioma)
        {
            default:
            case Idioma.Castellano:
                locale = "es";
                break;
            case Idioma.Ingles:
                locale = "en";
                break;
        }

        TranslationServer.SetLocale(locale);
    }

    /// <summary>
    /// Establece el idioma a castellano.
    /// </summary>
    public static void SetIdiomaCastellano()
    {
        SetIdioma(Idioma.Castellano);
    }

    /// <summary>
    /// Establece el idioma a inglés.
    /// </summary>
    public static void SetIdiomaIngles()
    {
        SetIdioma(Idioma.Ingles);
    }

    public static Idioma GetIdiomaActual()
    {
        string locale = TranslationServer.GetLocale();

        foreach (var kvp in IdiomasLocales)
        {
            if (kvp.Value == locale)
            {
                return kvp.Key;
            }
        }

        return Idioma.Castellano; // Valor por defecto
    }
}