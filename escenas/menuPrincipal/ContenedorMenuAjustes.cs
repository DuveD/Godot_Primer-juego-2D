using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.miscelaneo.controles;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuAjustes : CenterContainer
{
	private ControlSlider _ControlVolumenGeneral;
	public ControlSlider ControlVolumenGeneral => _ControlVolumenGeneral ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenGeneral");

	private ControlSlider _ControlVolumenMusica;
	private ControlSlider ControlVolumenMusica => _ControlVolumenMusica ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenMusica");

	private ControlSlider _ControlVolumenSonido;
	private ControlSlider ControlVolumenSonido => _ControlVolumenSonido ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenSonido");

	private ControlSeleccion _ControlLenguaje;
	private ControlSeleccion ControlLenguaje => _ControlLenguaje ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlLenguaje");

	private ControlSeleccion _ControlNivelLog;
	private ControlSeleccion ControlNivelLog => _ControlNivelLog ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlNivelLog");

	private Button _ButtonAtras;
	private Button ButtonAtras => _ButtonAtras ??= UtilidadesNodos.ObtenerNodoPorNombre<Button>(this, "ButtonAtras");

	private MenuPrincipal _MenuPrincipal;
	private MenuPrincipal MenuPrincipal => _MenuPrincipal ??= this.GetParent<MenuPrincipal>();

	private List<Control> ElementosMenuAjustes;

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		ConfigurarFocusElementos();

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

	private void ConfigurarFocusElementos()
	{
		LoggerJuego.Trace("Configuramos el focus de los elementos del menú ajustes.");

		ElementosMenuAjustes = [.. UtilidadesNodos.ObtenerNodosDeTipo<Button>(this)];
		ElementosMenuAjustes.AddRange(UtilidadesNodos.ObtenerNodosDeTipo<SpinBox>(this));
		ElementosMenuAjustes.AddRange(UtilidadesNodos.ObtenerNodosDeTipo<HSlider>(this));

		foreach (var elementoMenu in ElementosMenuAjustes)
		{
			elementoMenu.FocusEntered += () => this.MenuPrincipal.UltimoElementoConFocus = elementoMenu;
		}
	}

	public void ActivarNavegacionTeclado()
	{
		LoggerJuego.Trace("Activamos la navegación por teclado.");

		foreach (var elementoMenu in ElementosMenuAjustes)
			elementoMenu.FocusMode = FocusModeEnum.All;
	}

	public void DesactivarNavegacionTeclado()
	{
		LoggerJuego.Trace("Desactivamos la navegación por teclado.");

		foreach (var elementoMenu in ElementosMenuAjustes)
			elementoMenu.FocusMode = FocusModeEnum.None;
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