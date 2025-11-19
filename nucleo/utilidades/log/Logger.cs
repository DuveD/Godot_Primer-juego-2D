using System;
using System.Diagnostics;
using System.IO;
using Godot;
using Primerjuego2D.nucleo.ajustes;

namespace Primerjuego2D.nucleo.utilidades.log;

public static class Logger
{
    public const string FORMATO_FECHA_LOG = "yyyy-MM-dd HH:mm:ss";

    public enum LogLevel
    {
        Trace,
        Info,
        Warning,
        Error,
        None
    }

    public static LogLevel NivelLog = Ajustes.NivelLog;

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
        if (NivelLog > LogLevel.Trace) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("TRAZA", message, context, GD.Print);
    }

    public static void Info(string message, string context = "")
    {
        if (NivelLog > LogLevel.Info) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("INFO", message, context, GD.Print);
    }

    public static void Warn(string message, string context = "")
    {
        if (NivelLog > LogLevel.Warning) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("WARN", message, context, GD.PushWarning);
    }

    public static void Error(string message, string context = "")
    {
        if (NivelLog > LogLevel.Error) return;
        if (string.IsNullOrEmpty(context)) context = ObtenerContexto();
        EscribirLog("ERROR", message, context, GD.PrintErr);
    }

    // ================== Internals ==================

    private static string ObtenerContexto()
    {
        var frame = new StackTrace(true).GetFrame(2);
        var method = frame.GetMethod();

        string clase = method.DeclaringType?.Name ?? "UnknownClass";
        string metodo = method.Name;

        int linea = frame.GetFileLineNumber(); // ★ ← aquí obtenemos la línea

        // Si no hay info de PDB, devolver sin línea
        if (linea <= 0)
            return $"{clase}.{metodo}";

        return $"{clase}.{metodo}:{linea}";
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