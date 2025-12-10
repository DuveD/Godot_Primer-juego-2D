using Godot;
using System;
using Primerjuego2D.nucleo.utilidades;

public partial class ControlSlider : VBoxContainer
{
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
			SpinBox.Value = value;
			SliderVolumen.Value = value;
		}
	}

	private Label _label;
	private Label Label => _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

	private SpinBox _SpinBox;
	private SpinBox SpinBox => _SpinBox ??= UtilidadesNodos.ObtenerNodoDeTipo<SpinBox>(this);

	private HSlider _SliderVolumen;
	private HSlider SliderVolumen => _SliderVolumen ??= UtilidadesNodos.ObtenerNodoDeTipo<HSlider>(this);

	public override void _Ready()
	{
		// Configuración del rango del SpinBox
		SpinBox.MinValue = 0;
		SpinBox.MaxValue = 100;
		SpinBox.Step = 1;

		// Inicialmente sincronizamos ambos valores
		SpinBox.Value = SliderVolumen.Value;

		// Conectamos eventos
		SpinBox.ValueChanged += OnSpinBoxValueChanged;
		SliderVolumen.ValueChanged += OnSliderVolumenValueChanged;
	}

	private void OnSpinBoxValueChanged(double value)
	{
		// Evitamos bucle infinito
		if (SliderVolumen.Value != value)
		{
			SliderVolumen.Value = value;
			EmitSignal(SignalName.ValorCambiado);
		}
	}

	private void OnSliderVolumenValueChanged(double value)
	{
		if (SpinBox.Value != value)
		{
			SpinBox.Value = value;
			EmitSignal(SignalName.ValorCambiado, value);
		}
	}
}
