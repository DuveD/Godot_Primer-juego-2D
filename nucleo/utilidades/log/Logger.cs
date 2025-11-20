namespace Primerjuego2D.nucleo.utilidades.log;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.ajustes;

public static class Logger
{
    public enum NivelLog
    {
        Trace,
        Info,
        Warning,
        Error,
        None
    }

    public const string FORMATO_FECHA_LOG = "yyyy-MM-dd HH:mm:ss";


    public static bool EscribirLogEnFichero = Ajustes.EscribirLogEnFichero;

    private const int ContextWidth = 60;

    private static readonly string pathLog;

    static Logger()
    {
        string nombreJuego = (string)ProjectSettings.GetSetting("application/config/name");

        // Ruta a Documentos/MiJuego/logs/
        string pathMisDocumentos = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        string pathCarpetaLogs = Path.Combine(pathMisDocumentos, nombreJuego, "logs");
        if (!Directory.Exists(pathCarpetaLogs))
            Directory.CreateDirectory(pathCarpetaLogs);

        pathLog = Path.Combine(pathCarpetaLogs, $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

        Info("Log iniciado.", "Logger");
    }

    public static void Trace(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Trace) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("TRAZA", message, context, GD.Print);
    }

    public static void Info(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Info) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("INFO", message, context, GD.Print);
    }

    public static void Warn(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Warning) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("WARN", message, context, GD.PushWarning);
    }

    public static void Error(string message, string context = "")
    {
        NivelLog nivelLog = ObtenerNivelLog();
        if (nivelLog > NivelLog.Error) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("ERROR", message, context, GD.PrintErr);
    }

    // ================== Internals ==================

    private static NivelLog ObtenerNivelLog()
    {
        var frame = new StackTrace().GetFrame(2);
        var metodo = frame.GetMethod();
        Type tipoLlamador = metodo.DeclaringType;

        var atributoLogLevel = tipoLlamador.GetCustomAttributes(typeof(AtributoNivelLog), inherit: true)
                   .FirstOrDefault() as AtributoNivelLog;

        return atributoLogLevel?.NivelLog ?? Ajustes.NivelLog; // si no tiene atributo, usa NivelLog global
    }

    private static string ObtenerContexto()
    {
        var frame = new StackTrace(true).GetFrame(2);
        var metodo = frame.GetMethod();

        string clase = metodo.DeclaringType?.Name ?? "UnknownClass";
        string nombreMetodo = metodo.Name;

        int linea = frame.GetFileLineNumber();

        // Si no hay info de PDB, devolver sin línea.
        if (linea <= 0)
            return $"{clase}.{nombreMetodo}";

        return $"{clase}.{nombreMetodo}:{linea}";
    }

    private static void EscribirLog(string level, string message, string context, Action<string> consoleOutput)
    {
        string mensajeLog = FormatearMensajeLog(level, message, context);

        consoleOutput(mensajeLog);
        if (EscribirLogEnFichero)
            File.AppendAllText(pathLog, mensajeLog + "\n");
    }

    private static string FormatearMensajeLog(string level, string message, string context)
    {
        string time = DateTime.Now.ToString(FORMATO_FECHA_LOG);

        // Base del mensaje
        string mensajeBase = $"[{time}][{level}][{context}]: ";

        // Calculamos el ancho total deseado
        int widthContext = ContextWidth - mensajeBase.Length;

        // Creamos la variable para el padding (solo espacios)
        string padingMensaje = new string(' ', Math.Max(widthContext, 0));

        // Retornamos el mensaje final
        return $"{mensajeBase}{padingMensaje}{message}";
    }
}