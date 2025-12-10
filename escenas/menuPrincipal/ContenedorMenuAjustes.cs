using Godot;
using Primerjuego2D.escenas;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ContenedorMenuAjustes : CenterContainer
{
	private ControlSlider _ControlVolumenGeneral;
	private ControlSlider ControlVolumenGeneral => _ControlVolumenGeneral ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenGeneral");

	private ControlSlider _ControlVolumenMusica;
	private ControlSlider ControlVolumenMusica => _ControlVolumenMusica ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenMusica");

	private ControlSlider _ControlVolumenSonido;
	private ControlSlider ControlVolumenSonido => _ControlVolumenSonido ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenSonido");

	private ControlSeleccion _ControlLenguaje;
	private ControlSeleccion ControlLenguaje => _ControlLenguaje ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlLenguaje");

	public override void _Ready()
	{
		ControlVolumenGeneral.Valor = Global.GestorAudio.VolumenGeneral * 100.0;
		ControlVolumenMusica.Valor = Global.GestorAudio.VolumenMusica * 100.0;
		ControlVolumenSonido.Valor = Global.GestorAudio.VolumenSonidos * 100.0;

		Dictionary<Variant, string> opcionesLenguajes = GestorIdioma.IdiomasDisponibles.Values.ToDictionary(
			idioma => (Variant)idioma.Codigo,
			idioma => idioma.TagNombre
		);
		ControlLenguaje.AgregarOpciones(opcionesLenguajes);

		ControlVolumenGeneral.ValorCambiado += OnControlVolumenGeneralValorCambiado;
		ControlVolumenMusica.ValorCambiado += OnControlVolumenMusicaValorCambiado;
		ControlVolumenSonido.ValorCambiado += OnControlVolumenSonidosValorCambiado;
		ControlLenguaje.ValorCambiado += OnControlLenguajeValorCambiado;
	}

	private void OnControlVolumenGeneralValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenGeneral = (float)(valor / 100.0f);
		LoggerJuego.Info($"Volumen general ajustado a {valor}");
	}

	private void OnControlVolumenMusicaValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenMusica = (float)(valor / 100.0f);
		LoggerJuego.Info($"Volumen música ajustado a {valor}");
	}

	private void OnControlVolumenSonidosValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenSonidos = (float)(valor / 100.0f);
		LoggerJuego.Info($"Volumen sonidos ajustado a {valor}");
	}

	private void OnControlLenguajeValorCambiado(Variant valor)
	{
		string codigoIdioma = (string)valor;
		Idioma idiomaSeleccionado = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
		GestorIdioma.CambiarIdioma(idiomaSeleccionado);
		LoggerJuego.Info($"Idioma cambiado a {valor}");
	}
}
