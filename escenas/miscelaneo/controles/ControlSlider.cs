using System;
using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo.controles;

public partial class ControlSlider : VBoxContainer
{
	private bool _reproducirSonido = true;

	[Signal]
	public delegate void ValorCambiadoEventHandler(double valor);

	[Export]
	public string TextoLabel
	{
		get => Label.Text;
		set => Label.Text = value;
	}
	private double _valor;
	[Export]
	public double Valor
	{
		get => _valor;
		set
		{
			_valor = value;
			PutValueOnControl(value);
		}
	}

	private Label _label;
	public Label Label => _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

	private SpinBox _SpinBox;
	public SpinBox SpinBox => _SpinBox ??= UtilidadesNodos.ObtenerNodoDeTipo<SpinBox>(this);

	private HSlider _SliderVolumen;
	public HSlider SliderVolumen => _SliderVolumen ??= UtilidadesNodos.ObtenerNodoDeTipo<HSlider>(this);

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		// Configuración del rango del SpinBox

		this.SpinBox.MinValue = 0;
		this.SpinBox.MaxValue = 100;
		this.SpinBox.Step = 1;

		this.SliderVolumen.MinValue = 0;
		this.SliderVolumen.MaxValue = 100;
		this.SliderVolumen.Step = 1;

		// Inicialmente sincronizamos ambos valores

		this.SpinBox.Value = SliderVolumen.Value;

		// Conectamos eventos

		this.SpinBox.ValueChanged += OnSpinBoxValueChanged;
		this.SliderVolumen.ValueChanged += OnSliderVolumenValueChanged;

		this.SpinBox.MouseEntered += OnMouseEntered;
		this.SliderVolumen.FocusEntered += OnFocusedEntered;
		this.SliderVolumen.MouseEntered += OnMouseEntered;
	}

	public void OnFocusedEntered()
	{
		if (this._reproducirSonido)
			Global.GestorAudio.ReproducirSonido("kick.mp3");
	}

	private void OnMouseEntered()
	{
		Global.GestorAudio.ReproducirSonido("kick.mp3");
	}

	public void GrabFocusSilencioso()
	{
		this._reproducirSonido = false;
		this.SliderVolumen.GrabFocus();
		this._reproducirSonido = true;
	}

	private void OnSpinBoxValueChanged(double value)
	{
		// Evitamos bucle infinito

		if (SliderVolumen.Value != value)
		{
			if (!Equals(_valor, value))
			{
				SliderVolumen.Value = value;
				EmitSignal(SignalName.ValorCambiado, value);
			}
		}
	}

	private void OnSliderVolumenValueChanged(double value)
	{
		if (SpinBox.Value != value)
		{
			if (!Equals(_valor, value))
			{
				SpinBox.Value = value;
				EmitSignal(SignalName.ValorCambiado, value);
			}
		}
	}

	private void PutValueOnControl(double value)
	{
		SpinBox.Value = value;
		SliderVolumen.Value = value;
	}
}