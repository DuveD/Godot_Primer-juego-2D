using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.miscelaneo.controles;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.modelos;
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

	private ButtonPersonalizado _ButtonAtras;
	private ButtonPersonalizado ButtonAtras => _ButtonAtras ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

	private ButtonPersonalizado _ButtonGuardar;
	private ButtonPersonalizado ButtonGuardar => _ButtonGuardar ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonGuardar");

	private MenuPrincipal _MenuPrincipal;
	private MenuPrincipal MenuPrincipal => _MenuPrincipal ??= this.GetParent<MenuPrincipal>();

	private List<Control> ElementosMenuAjustes;

	// Ajustes actuales.
	public double VolumenGeneral;
	public double VolumenMusica;
	public double VolumenSonidos;
	public Idioma Lenguaje;
	public NivelLog NivelLog;

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		ConfigurarFocusElementos();

		CargarOpcionesLenguaje();
		CargarOpcionesNivelLog();

		// Cargar ajustes actuales.
		CargarValoresDeAjustes();
	}

	private void CargarValoresDeAjustes()
	{
		ControlVolumenGeneral.ValorCambiado -= OnControlVolumenGeneralValorCambiado;
		ControlVolumenMusica.ValorCambiado -= OnControlVolumenMusicaValorCambiado;
		ControlVolumenSonido.ValorCambiado -= OnControlVolumenSonidosValorCambiado;
		ControlLenguaje.ValorCambiado -= OnControlLenguajeValorCambiado;
		ControlNivelLog.ValorCambiado -= OnControlNivelLogValorCambiado;

		VolumenGeneral = Ajustes.VolumenGeneral;
		ControlVolumenGeneral.Valor = VolumenGeneral * 100.0;

		VolumenMusica = Ajustes.VolumenMusica;
		ControlVolumenMusica.Valor = VolumenMusica * 100.0;

		VolumenSonidos = Ajustes.VolumenSonidos;
		ControlVolumenSonido.Valor = VolumenSonidos * 100.0;

		Lenguaje = Ajustes.Idioma;
		ControlLenguaje.Valor = Ajustes.Idioma.Codigo;

		NivelLog = Ajustes.NivelLog;
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
			var elemento = elementoMenu;
			elemento.FocusEntered += () => MenuPrincipal.UltimoElementoConFocus = elemento;
		}
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

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
		{
			OnScapeButtonPressed();
		}
	}

	private void OnScapeButtonPressed()
	{
		if (this.Visible)
			UtilidadesNodos.PulsarBoton(ButtonAtras);
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

	private void ActivarBotonGuardarSiCambio()
	{
		bool hayCambios =
			!VolumenGeneral.Equals(ControlVolumenGeneral.Valor / 100.0) ||
			!VolumenMusica.Equals(ControlVolumenMusica.Valor / 100.0) ||
			!VolumenSonidos.Equals(ControlVolumenSonido.Valor / 100.0) ||
			!Lenguaje.Codigo.Equals(ControlLenguaje.Valor.AsString()) ||
			!NivelLog.Equals((NivelLog)(int)ControlNivelLog.Valor);

		ButtonGuardar.Disabled = !hayCambios;
	}

	private void OnControlVolumenGeneralValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenGeneral = (float)(valor / 100.0f);
		ActivarBotonGuardarSiCambio();
	}

	private void OnControlVolumenMusicaValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenMusica = (float)(valor / 100.0f);
		ActivarBotonGuardarSiCambio();
	}

	private void OnControlVolumenSonidosValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenSonidos = (float)(valor / 100.0f);
		ActivarBotonGuardarSiCambio();
	}

	private void OnControlLenguajeValorCambiado(Variant valor)
	{
		string codigoIdioma = (string)valor;
		Idioma idiomaSeleccionado = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
		GestorIdioma.CambiarIdioma(idiomaSeleccionado);
		ActivarBotonGuardarSiCambio();
	}

	private void OnControlNivelLogValorCambiado(Variant valor)
	{
		NivelLog nivelLogSeleccionado = (NivelLog)(int)valor;
		LoggerJuego.NivelLogJuego = nivelLogSeleccionado;
		ActivarBotonGuardarSiCambio();
	}


	public void OnButtonGuardarPressed()
	{
		LoggerJuego.Trace("Botón Ajustes 'Guardar' pulsado.");

		Ajustes.GuardarAjustesAlGuardarPropiedad = false;
		Ajustes.VolumenGeneral = (float)(ControlVolumenGeneral.Valor / 100.0);
		Ajustes.VolumenMusica = (float)(ControlVolumenMusica.Valor / 100.0);
		Ajustes.VolumenSonidos = (float)(ControlVolumenSonido.Valor / 100.0);
		Ajustes.Idioma = GestorIdioma.ObtenerIdiomaDeCodigo((string)ControlLenguaje.Valor);
		Ajustes.NivelLog = (NivelLog)(int)ControlNivelLog.Valor;

		Ajustes.GuardarAjustes();
		Ajustes.GuardarAjustesAlGuardarPropiedad = true;

		CargarValoresDeAjustes();

		ButtonGuardar.Disabled = true;
	}
}