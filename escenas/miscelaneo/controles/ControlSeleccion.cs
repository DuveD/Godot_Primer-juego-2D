using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo.controles;

public partial class ControlSeleccion : HBoxContainer
{
	private bool _reproducirSonido = true;

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
			PutValueOnControl(value);
		}
	}

	private Label _Label;
	public Label Label => _Label ??= UtilidadesNodos.ObtenerNodoDeTipo<Label>(this);

	private OptionButton _OptionButton;
	public OptionButton OptionButton => _OptionButton ??= UtilidadesNodos.ObtenerNodoDeTipo<OptionButton>(this);

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		this.OptionButton.ItemSelected += OnOptionButtonItemSelected;

		this.OptionButton.FocusEntered += OnFocusedEntered;
		this.OptionButton.MouseEntered += OnMouseEntered;
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
		this.OptionButton.GrabFocus();
		this._reproducirSonido = true;
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

	private void PutValueOnControl(Variant valor)
	{
		for (int i = 0; i < OptionButton.GetItemCount(); i++)
		{
			if (OptionButton.GetItemMetadata(i).EqualsByType(valor))
			{
				OptionButton.Select(i);
				return;
			}
		}
		throw new ArgumentException("Valor no encontrado en las opciones del ControlSeleccion.");
	}

	private void OnOptionButtonItemSelected(long index)
	{
		var valorSeleleccionado = OptionButton.GetItemMetadata((int)index);
		if (!Equals(_valor, valorSeleleccionado))
		{
			_valor = valorSeleleccionado;
			EmitSignal(SignalName.ValorCambiado, _valor);
		}
	}
}