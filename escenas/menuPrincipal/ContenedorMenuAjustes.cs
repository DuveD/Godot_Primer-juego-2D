using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;
using ButtonPersonalizado = Primerjuego2D.escenas.modelos.controles.ButtonPersonalizado;
using ControlSeleccion = Primerjuego2D.escenas.modelos.controles.ControlSeleccion;
using ControlSlider = Primerjuego2D.escenas.modelos.controles.ControlSlider;

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
	public int VolumenGeneral;
	public int VolumenMusica;
	public int VolumenSonidos;
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
		ControlVolumenGeneral.Valor = VolumenGeneral;

		VolumenMusica = Ajustes.VolumenMusica;
		ControlVolumenMusica.Valor = VolumenMusica;

		VolumenSonidos = Ajustes.VolumenSonidos;
		ControlVolumenSonido.Valor = VolumenSonidos;

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
			!VolumenGeneral.Equals((int)ControlVolumenGeneral.Valor) ||
			!VolumenMusica.Equals((int)ControlVolumenMusica.Valor) ||
			!VolumenSonidos.Equals((int)ControlVolumenSonido.Valor) ||
			!Lenguaje.Codigo.Equals(ControlLenguaje.Valor.AsString()) ||
			!NivelLog.Equals((NivelLog)(int)ControlNivelLog.Valor);

		if (hayCambios)
		{
			ActivarNavegacionButtonGuardar();
		}
		else
		{
			DesactivarNavegacionButtonGuardar();
		}
	}

	private void ActivarNavegacionButtonGuardar()
	{
		ButtonGuardar.Disabled = false;
		ButtonGuardar.FocusMode = FocusModeEnum.All;

		// Informamos al ControlNivelLog que su vecino de abajo es el botón Guardar
		ControlNivelLog.OptionButton.FocusNeighborBottom = ControlNivelLog.OptionButton.GetPathTo(ButtonGuardar);
		// Informamos al botón Atrás que su vecino a la derecha es el botón Guardar
		ButtonAtras.FocusNeighborRight = ButtonAtras.GetPathTo(ButtonGuardar);
	}

	private void DesactivarNavegacionButtonGuardar()
	{
		ButtonGuardar.Disabled = true;
		ButtonGuardar.FocusMode = FocusModeEnum.None;

		// Informamos al ControlNivelLog que su vecino de abajo es el botón Atrás
		ControlNivelLog.OptionButton.FocusNeighborBottom = ControlNivelLog.OptionButton.GetPathTo(ButtonAtras);
		// Informamos al botón Atrás que su vecino a la derecha es el ControlNivelLog
		ButtonAtras.FocusNeighborRight = ButtonAtras.GetPathTo(ControlNivelLog.OptionButton);
	}

	private void OnControlVolumenGeneralValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenGeneral = (float)(valor / 100.0f);
		//ActivarBotonGuardarSiCambio();
		Ajustes.VolumenGeneral = (int)valor;
	}

	private void OnControlVolumenMusicaValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenMusica = (float)(valor / 100.0f);
		//ActivarBotonGuardarSiCambio();
		Ajustes.VolumenMusica = (int)valor;
	}

	private void OnControlVolumenSonidosValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenSonidos = (float)(valor / 100.0f);
		//ActivarBotonGuardarSiCambio();
		Ajustes.VolumenSonidos = (int)valor;
	}

	private void OnControlLenguajeValorCambiado(Variant valor)
	{
		string codigoIdioma = (string)valor;
		Idioma idiomaSeleccionado = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
		GestorIdioma.CambiarIdioma(idiomaSeleccionado);
		//ActivarBotonGuardarSiCambio();
		Ajustes.Idioma = idiomaSeleccionado;
	}

	private void OnControlNivelLogValorCambiado(Variant valor)
	{
		NivelLog nivelLogSeleccionado = (NivelLog)(int)valor;
		LoggerJuego.NivelLogJuego = nivelLogSeleccionado;
		//ActivarBotonGuardarSiCambio();
		Ajustes.NivelLog = nivelLogSeleccionado;
	}

	public void OnButtonGuardarPressed()
	{
		LoggerJuego.Trace("Botón Ajustes 'Guardar' pulsado.");

		Ajustes.GuardarAjustesAlGuardarPropiedad = false;
		Ajustes.VolumenGeneral = (int)ControlVolumenGeneral.Valor;
		Ajustes.VolumenMusica = (int)ControlVolumenMusica.Valor;
		Ajustes.VolumenSonidos = (int)ControlVolumenSonido.Valor;
		Ajustes.Idioma = GestorIdioma.ObtenerIdiomaDeCodigo((string)ControlLenguaje.Valor);
		Ajustes.NivelLog = (NivelLog)(int)ControlNivelLog.Valor;

		Ajustes.GuardarAjustes();
		Ajustes.GuardarAjustesAlGuardarPropiedad = true;

		CargarValoresDeAjustes();

		DesactivarNavegacionButtonGuardar();

		ButtonAtras.GrabFocusSilencioso();
	}
}