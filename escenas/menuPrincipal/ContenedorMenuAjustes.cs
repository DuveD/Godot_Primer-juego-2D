using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

public partial class ContenedorMenuAjustes : CenterContainer
{
	private Primerjuego2D.escenas.miscelaneo.controles.ControlSlider _ControlVolumenGeneral;
	private Primerjuego2D.escenas.miscelaneo.controles.ControlSlider ControlVolumenGeneral => _ControlVolumenGeneral ??= UtilidadesNodos.ObtenerNodoPorNombre<Primerjuego2D.escenas.miscelaneo.controles.ControlSlider>(this, "ControlVolumenGeneral");

	private Primerjuego2D.escenas.miscelaneo.controles.ControlSlider _ControlVolumenMusica;
	private Primerjuego2D.escenas.miscelaneo.controles.ControlSlider ControlVolumenMusica => _ControlVolumenMusica ??= UtilidadesNodos.ObtenerNodoPorNombre<Primerjuego2D.escenas.miscelaneo.controles.ControlSlider>(this, "ControlVolumenMusica");

	private Primerjuego2D.escenas.miscelaneo.controles.ControlSlider _ControlVolumenSonido;
	private Primerjuego2D.escenas.miscelaneo.controles.ControlSlider ControlVolumenSonido => _ControlVolumenSonido ??= UtilidadesNodos.ObtenerNodoPorNombre<Primerjuego2D.escenas.miscelaneo.controles.ControlSlider>(this, "ControlVolumenSonido");

	private Primerjuego2D.escenas.miscelaneo.controles.ControlSeleccion _ControlLenguaje;
	private Primerjuego2D.escenas.miscelaneo.controles.ControlSeleccion ControlLenguaje => _ControlLenguaje ??= UtilidadesNodos.ObtenerNodoPorNombre<Primerjuego2D.escenas.miscelaneo.controles.ControlSeleccion>(this, "ControlLenguaje");

	private Primerjuego2D.escenas.miscelaneo.controles.ControlSeleccion _ControlNivelLog;
	private Primerjuego2D.escenas.miscelaneo.controles.ControlSeleccion ControlNivelLog => _ControlNivelLog ??= UtilidadesNodos.ObtenerNodoPorNombre<Primerjuego2D.escenas.miscelaneo.controles.ControlSeleccion>(this, "ControlNivelLog");
	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		CargarOpcionesLenguaje();
		CargarOpcionesNivelLog();

		ControlVolumenGeneral.Valor = Ajustes.VolumenGeneral * 100.0;
		ControlVolumenMusica.Valor = Ajustes.VolumenMusica * 100.0;
		ControlVolumenSonido.Valor = Ajustes.VolumenSonidos * 100.0;
		ControlLenguaje.Valor = Ajustes.Idioma.Codigo;
		ControlNivelLog.Valor = (int)Ajustes.NivelLog;

		ControlVolumenGeneral.ValorCambiado += OnControlVolumenGeneralValorCambiado;
		ControlVolumenMusica.ValorCambiado += OnControlVolumenMusicaValorCambiado;
		ControlVolumenSonido.ValorCambiado += OnControlVolumenSonidosValorCambiado;
		ControlLenguaje.ValorCambiado += OnControlLenguajeValorCambiado;
		ControlNivelLog.ValorCambiado += OnControlNivelLogValorCambiado;
	}

	private void CargarOpcionesLenguaje()
	{
		var opcionesLenguajes = GestorIdioma.IdiomasDisponibles.Values.ToDictionary(
			idioma => (Variant)idioma.Codigo,
			idioma => idioma.TagNombre
		);
		ControlLenguaje.AgregarOpciones(opcionesLenguajes);
	}

	private void CargarOpcionesNivelLog()
	{
		var opcionesNivelLog = Enum.GetValues<NivelLog>().ToDictionary(
			nivel => (Variant)(int)nivel,
			nivel => nivel.ToString()
		);
		ControlNivelLog.AgregarOpciones(opcionesNivelLog);
	}

	public void OnButtonGuardarPressed()
	{
		LoggerJuego.Trace("Botón Ajustes 'Guardar' pulsado.");

		Global.GestorAudio.ReproducirSonido("digital_click.mp3");

		Ajustes.GuardarAjustes();
	}

	private void OnControlVolumenGeneralValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenGeneral = (float)(valor / 100.0f);
		LoggerJuego.Trace($"Volumen general ajustado a {valor}");
	}

	private void OnControlVolumenMusicaValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenMusica = (float)(valor / 100.0f);
		LoggerJuego.Trace($"Volumen música ajustado a {valor}");
	}

	private void OnControlVolumenSonidosValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenSonidos = (float)(valor / 100.0f);
		LoggerJuego.Trace($"Volumen sonidos ajustado a {valor}");
	}

	private void OnControlLenguajeValorCambiado(Variant valor)
	{
		string codigoIdioma = (string)valor;
		Idioma idiomaSeleccionado = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
		GestorIdioma.CambiarIdioma(idiomaSeleccionado);
		Ajustes.Idioma = idiomaSeleccionado;
		LoggerJuego.Trace($"Idioma cambiado a {valor}");
	}

	private void OnControlNivelLogValorCambiado(Variant valor)
	{
		NivelLog nivelLog = (NivelLog)(int)valor;
		Ajustes.NivelLog = nivelLog;
		LoggerJuego.Trace($"Nivel de log cambiado a {nivelLog}");
	}
}
