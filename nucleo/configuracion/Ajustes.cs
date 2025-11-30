using Godot;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.nucleo.configuracion;

public static class Ajustes
{
    // ================= SECCIONES =================

    public const string SECCION_SONIDO = "sonido";
    public const string SECCION_INTERFAZ = "interfaz";
    public const string SECCION_DESARROLLO = "desarrollo";

    // ================= INTERNOS =================

    public static string NombreJuego { get; } = (string)ProjectSettings.GetSetting("application/config/name");
    public static string RutaMisDocumentos { get; } = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    public static string RutaJuego { get; } = $"{RutaMisDocumentos}/{NombreJuego}";
    public static string RutaLogs { get; } = $"{RutaJuego}/logs";
    public static string NombreArchivoAjustes { get; } = "ajustes.ini";
    public static string RutaArchivoAjustes { get; } = $"{RutaJuego}/{NombreArchivoAjustes}";

    // ================= SONIDO =================
    public static float VolumenGeneral
    {
        get => GestorAjustes.ObtenerValor(SECCION_SONIDO, "volumen_general", 1.0f);
        set => GuardarPropiedad(SECCION_SONIDO, "volumen_general", value);
    }

    public static float VolumenMusica
    {
        get => GestorAjustes.ObtenerValor(SECCION_SONIDO, "volumen_musica", 1.0f);
        set => GuardarPropiedad(SECCION_SONIDO, "volumen_musica", value);
    }

    public static float VolumenSonidos
    {
        get => GestorAjustes.ObtenerValor(SECCION_SONIDO, "volumen_sonidos", 1.0f);
        set => GuardarPropiedad(SECCION_SONIDO, "volumen_sonidos", value);
    }

    // ================= INTERFAZ =================

    public static Idioma Idioma
    {
        get
        {
            string codigo = GestorAjustes.ObtenerValor(SECCION_INTERFAZ, "idioma", Idioma.ES.Codigo);
            var idioma = GestorIdioma.ObtenerIdiomaDeCodigo(codigo);
            return idioma ?? Idioma.ES;
        }

        set
        {
            string valor = value.Codigo;
            GuardarPropiedad(SECCION_INTERFAZ, "idioma", valor);
        }
    }

    // ================= DESARROLLO =================

    public static NivelLog NivelLog
    {
        get => GestorAjustes.ObtenerValor(SECCION_DESARROLLO, "nivel_log", NivelLog.Trace);
        set => GuardarPropiedad(SECCION_DESARROLLO, "nivel_log", value);
    }

    public static bool EscribirLogEnFichero
    {
        get => GestorAjustes.ObtenerValor(SECCION_DESARROLLO, "escribir_log_en_fichero", true);
        set => GuardarPropiedad(SECCION_DESARROLLO, "escribir_log_en_fichero", value);
    }

    // ================= CARGA Y GUARDADO =================

    private static bool _guardarAjustesAlGuardarPropiedad = true;

    private static void InicializarValoresPorDefecto()
    {
        _guardarAjustesAlGuardarPropiedad = false;

        VolumenGeneral = 1.0f;
        VolumenMusica = 1.0f;
        VolumenSonidos = 1.0f;

        Idioma = Idioma.ES;
        NivelLog = NivelLog.Trace;
        EscribirLogEnFichero = false;

        _guardarAjustesAlGuardarPropiedad = true;
    }

    public static void CargarAjustes()
    {
        if (!FileAccess.FileExists(RutaArchivoAjustes))
        {
            InicializarValoresPorDefecto();

            GestorAjustes.Guardar(RutaArchivoAjustes);

            LoggerJuego.Info($"Creado archivo '{NombreArchivoAjustes}' con la configuración por defecto.");
        }
        else
        {
            GestorAjustes.Cargar(RutaArchivoAjustes);
            LoggerJuego.Info("Ajustes cargados.");
        }
    }

    public static void GuardarPropiedad<[MustBeVariant] T>(string seccion, string clave, T valor)
    {
        GestorAjustes.GuardarValor(seccion, clave, valor);
        if (_guardarAjustesAlGuardarPropiedad)
            GuardarAjustes();
    }


    public static void GuardarAjustes()
    {
        GestorAjustes.Guardar(RutaArchivoAjustes);
        LoggerJuego.Info("Ajustes guardados.");
    }
}