using Godot;
using Primerjuego2D.nucleo.utilidades;
using System;
using System.Collections.Generic;

public partial class ControlSeleccion : HBoxContainer
{
	[Signal]
	public delegate void ValorCambiadoEventHandler(Variant valor);

	[Export]
	public string TextoLabel
	{
		get => Label.Text;
		set => Label.Text = value;
	}

	private Variant _valor;

	[Export]
	public Variant Valor
	{
		get => _valor;
		set
		{
			_valor = value;
		}
	}

	private Label _label;
	private Label Label => _label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

	private OptionButton _OptionButton;
	private OptionButton OptionButton => _OptionButton ??= UtilidadesNodos.ObtenerNodoDeTipo<OptionButton>(this);

	public override void _Ready()
	{
		this.OptionButton.ItemSelected += OnOptionButtonItemSelected;
	}

	public void AgregarOpciones(Dictionary<Variant, string> opciones)
	{
		foreach (var kvp in opciones)
		{
			AgregarOpcion(kvp.Key, kvp.Value);
		}
	}

	public void AgregarOpcion(Variant valor, string tagTexto)
	{
		OptionButton.AddItem(tagTexto);
		int index = OptionButton.GetItemCount() - 1;
		OptionButton.SetItemMetadata(index, valor);
	}

	public void SetValor(Variant valor)
	{
		for (int i = 0; i < OptionButton.GetItemCount(); i++)
		{
			if (OptionButton.GetItemMetadata(i).Equals(valor))
			{
				OptionButton.Select(i);
				_valor = valor;
				return;
			}
		}
		throw new ArgumentException("Valor no encontrado en las opciones del ControlSeleccion.");
	}

	private void OnOptionButtonItemSelected(long index)
	{
		_valor = OptionButton.GetItemMetadata((int)index);
		EmitSignal(SignalName.ValorCambiado, _valor);
	}
}
