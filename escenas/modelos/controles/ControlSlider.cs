using System;
using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.modelos.controles;

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

	[Export]
	public bool MostrarValorNumerico
	{
		get => SpinBox.Visible;
		set => SpinBox.Visible = value;
	}

	public bool _ModoEntero = true;
	[Export]
	public bool ModoEntero
	{
		get => _ModoEntero;
		set => _ModoEntero = value;
	}

	private double _valor;

	[Export]
	public double Valor
	{
		get => _valor;
		set => SetValorInterno(value);
	}

	private double _MinValor = 0;

	[Export]
	public double MinValor
	{
		get => _MinValor;
		set
		{
			_MinValor = value;
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
			_step = ModoEntero ? Math.Max(1, Math.Truncate(value)) : value;
			AplicarRango();
		}
	}

	private Label _label;
	public Label Label => _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

	private SpinBox _spinBox;
	public SpinBox SpinBox => _spinBox ??= UtilidadesNodos.ObtenerNodoDeTipo<SpinBox>(this);

	private HSliderPersonalizado _slider;
	public HSliderPersonalizado SliderVolumen =>
		_slider ??= UtilidadesNodos.ObtenerNodoDeTipo<HSliderPersonalizado>(this);


	public override void _Ready()
	{
		LoggerJuego.Trace($"{Name} Ready.");

		AplicarRango();
		SetValorInterno(_valor, emitirSenal: false);

		SpinBox.ValueChanged += OnSpinBoxValueChanged;
		SliderVolumen.ValueChanged += OnSliderValueChanged;
	}

	private void OnSpinBoxValueChanged(double value)
	{
		SetValorInterno(value);
	}

	private void OnSliderValueChanged(double value)
	{
		SetValorInterno(value);
	}

	public void SetValorInterno(double value, bool emitirSenal = true)
	{
		if (NearlyEqual(_valor, value))
			return;

		_valor = this.ModoEntero ? Math.Truncate(value) : value;

		// Sincronizamos controles sin provocar bucles

		if (!NearlyEqual(SpinBox.Value, _valor))
			SpinBox.Value = _valor;

		if (!NearlyEqual(SliderVolumen.Value, _valor))
			SliderVolumen.Value = _valor;

		if (emitirSenal)
			EmitSignal(SignalName.ValorCambiado, _valor);
	}

	private void AplicarRango()
	{
		if (_spinBox == null || _slider == null)
			return;

		SpinBox.MinValue = _MinValor;
		SpinBox.MaxValue = _maxValor;
		SpinBox.Step = _step;

		SliderVolumen.MinValue = _MinValor;
		SliderVolumen.MaxValue = _maxValor;
		SliderVolumen.Step = _step;

		// Aseguramos que el valor actual sigue siendo válido

		SetValorInterno(Math.Clamp(_valor, _MinValor, _maxValor), emitirSenal: false);
	}

	private static bool NearlyEqual(double a, double b, double epsilon = 0.0001)
	{
		return Math.Abs(a - b) < epsilon;
	}
}