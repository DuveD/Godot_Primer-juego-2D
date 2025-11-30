using System;
using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.configuracion;

/// <summary>
/// Clase encargada de la persistencia de ajustes en ConfigFile.
/// </summary>
public static class GestorAjustes
{
    private static ConfigFile Archivo { get; } = new ConfigFile();

    // ---------------- Carga y guardado ----------------
    public static void Cargar(string rutaArchivo)
    {
        if (!FileAccess.FileExists(rutaArchivo))
            return;

        var err = Archivo.Load(rutaArchivo);
        if (err != Error.Ok)
            LoggerJuego.EscribirLog("ERROR", $"No se pudo cargar el archivo de ajustes: {rutaArchivo}", null, "red");
    }

    public static void Guardar(string rutaArchivo)
    {
        var err = Archivo.Save(rutaArchivo);
        if (err != Error.Ok)
            LoggerJuego.EscribirLog("ERROR", $"No se pudo guardar el archivo de ajustes: {rutaArchivo}", null, "red");
    }

    // ---------------- Métodos genéricos ----------------
    public static void GuardarValor<[MustBeVariant] T>(string seccion, string clave, T valor)
    {
        Variant valorGuardar;

        if (typeof(T).IsEnum)
        {
            valorGuardar = valor?.ToString()?.ToLower();
        }
        else
        {
            valorGuardar = Variant.From(valor);
        }

        Archivo.SetValue(seccion, clave, valorGuardar);
    }

    public static T ObtenerValor<[MustBeVariant] T>(string seccion, string clave, T valorPorDefecto = default)
    {
        T valor;

        Variant valorGuardado = Archivo.GetValue(seccion, clave);

        if (valorGuardado.VariantType == Variant.Type.Nil)
            return valorPorDefecto;

        if (typeof(T).IsEnum)
        {
            if (Enum.TryParse(typeof(T), valorGuardado.ToString(), ignoreCase: true, out object enumRes))
                valor = (T)enumRes;
            else
                valor = valorPorDefecto;
        }
        else
        {
            valor = valorGuardado.As<T>();
        }

        return valor;
    }
}