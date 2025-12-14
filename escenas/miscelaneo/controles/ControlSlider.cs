using System;
using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo.controles;

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
		set => SetValorInterno(value);
	}

	private double _minValor = 0;

	[Export]
	public double MinValor
	{
		get => _minValor;
		set
		{
			_minValor = value;
			AplicarRango();
		}
	}

	private double _maxValor = 100;

	[Export]
	public double MaxValor
	{
		get => _maxValor;
		set
		{
			_maxValor = value;
			AplicarRango();
		}
	}

	private double _step = 1;

	[Export]
	public double Step
	{
		get => _step;
		set
		{
			_step = value;
			AplicarRango();
		}
	}

	private Label _label;
	public Label Label => _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

	private SpinBox _spinBox;
	public SpinBox SpinBox => _spinBox ??= UtilidadesNodos.ObtenerNodoDeTipo<SpinBox>(this);

	private nucleo.modelos.HSliderPersonalizado _slider;
	public nucleo.modelos.HSliderPersonalizado SliderVolumen =>
		_slider ??= UtilidadesNodos.ObtenerNodoDeTipo<nucleo.modelos.HSliderPersonalizado>(this);


	public override void _Ready()
	{
		LoggerJuego.Trace($"{Name} Ready.");

		AplicarRango();
		SetValorInterno(_valor, emitirSenal: false);

		SpinBox.ValueChanged += OnSpinBoxValueChanged;
		SliderVolumen.ValueChanged += OnSliderValueChanged;
	}

	#region Event handlers


	private void OnSpinBoxValueChanged(double value)
	{
		SetValorInterno(value);
	}

	private void OnSliderValueChanged(double value)
	{
		SetValorInterno(value);
	}

	#endregion

	#region Internal logic


	public void SetValorInterno(double value, bool emitirSenal = true)
	{
		if (NearlyEqual(_valor, value))
			return;

		_valor = value;

		// Sincronizamos controles sin provocar bucles

		if (!NearlyEqual(SpinBox.Value, value))
			SpinBox.Value = value;

		if (!NearlyEqual(SliderVolumen.Value, value))
			SliderVolumen.Value = value;

		if (emitirSenal)
			EmitSignal(SignalName.ValorCambiado, value);
	}

	private void AplicarRango()
	{
		if (_spinBox == null || _slider == null)
			return;

		SpinBox.MinValue = _minValor;
		SpinBox.MaxValue = _maxValor;
		SpinBox.Step = _step;

		SliderVolumen.MinValue = _minValor;
		SliderVolumen.MaxValue = _maxValor;
		SliderVolumen.Step = _step;

		// Aseguramos que el valor actual sigue siendo válido

		SetValorInterno(Math.Clamp(_valor, _minValor, _maxValor), emitirSenal: false);
	}

	private static bool NearlyEqual(double a, double b, double epsilon = 0.0001)
	{
		return Math.Abs(a - b) < epsilon;
	}


	#endregion
}