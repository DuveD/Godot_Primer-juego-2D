using System.IO;
using Godot;
using Primerjuego2D.nucleo.modelos.estadisticas;

namespace Primerjuego2D.nucleo.configuracion;

public class GestorEstadisticas
{
    private const string SECCION_ESTADISTICAS = "estadisticas";

    private ConfigFile ArchivoEstadisticas { get; } = new ConfigFile();

    public EstadisticasPartida PartidaActual { get; private set; }
    public EstadisticasGlobales Globales { get; private set; }

    public GestorEstadisticas()
    {
        CargarGlobales();
        NuevaPartida();
    }

    private void CargarGlobales()
    {
        if (File.Exists(Ajustes.RutaArchivoEstadisticas))
        {
            var err = this.ArchivoEstadisticas.Load(Ajustes.RutaArchivoEstadisticas);
            if (err != Error.Ok)
            {
                this.Globales = new EstadisticasGlobales();
            }
            else
            {
                this.Globales = new EstadisticasGlobales
                {
                    PartidasJugadas = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "partidas_jugadas", 0),
                    MejorPuntuacion = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "mejor_puntuacion", 0),
                    MonedasRecogidas = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "monedas_recogidas", 0),
                    MonedasEspecialesRecogidas = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "monedas_especiales_recogidas", 0),
                    EnemigosDerrotados = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "enemigos_derrotados", 0)
                };
            }
        }
        else
        {
            this.Globales = new EstadisticasGlobales();
        }
    }

    private void NuevaPartida()
    {
        this.PartidaActual = new EstadisticasPartida();
    }

    public void RegistrarMoneda(bool especial)
    {
        this.PartidaActual.MonedasRecogidas++;
        if (especial)
            this.PartidaActual.MonedasEspecialesRecogidas++;
    }

    public void FinalizarPartida(int puntos)
    {
        this.PartidaActual.PuntuacionFinal += puntos;

        ActualizarGlobales();
        GuardarGlobales();
    }

    private void ActualizarGlobales()
    {
        this.Globales.PartidasJugadas++;
        this.Globales.MonedasRecogidas += this.PartidaActual.MonedasRecogidas;
        this.Globales.MonedasEspecialesRecogidas += this.PartidaActual.MonedasEspecialesRecogidas;
        this.Globales.EnemigosDerrotados += this.PartidaActual.EnemigosDerrotados;

        if (this.PartidaActual.PuntuacionFinal > this.Globales.MejorPuntuacion)
            this.Globales.MejorPuntuacion = this.PartidaActual.PuntuacionFinal;
    }

    private void GuardarGlobales()
    {
        this.ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "partidas_jugadas", this.Globales.PartidasJugadas);
        this.ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "mejor_puntuacion", this.Globales.MejorPuntuacion);
        this.ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "monedas_recogidas", this.Globales.MonedasRecogidas);
        this.ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "monedas_especiales_recogidas", this.Globales.MonedasEspecialesRecogidas);
        this.ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "enemigos_derrotados", this.Globales.EnemigosDerrotados);

        if (!Directory.Exists(Ajustes.RutaJuego))
            Directory.CreateDirectory(Ajustes.RutaJuego);

        var err = this.ArchivoEstadisticas.Save(Ajustes.RutaArchivoEstadisticas);
        if (err != Error.Ok)
            GD.PrintErr($"No se pudo guardar el archivo de estadísticas: {err}");
    }
}
