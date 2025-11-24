using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Primerjuego2D.escenas.objetos.moneda;

public partial class SpawnMonedas : Control
{
	public int MonedasRecogidas;

	// Señal "MonedaRecogida" para indicar que el jugador ha recogido una moneda.
	[Signal]
	public delegate void MonedaRecogidaEventHandler();

	[Export] public PackedScene MonedaScene;

	public override void _Ready()
	{
		this.MonedasRecogidas = 0;

		Spawn();
	}

	public override void _Process(double delta)
	{
		if (!ExisteMoneda())
			Spawn();
	}

	public bool ExisteMoneda()
	{
		var monedas = GetTree().CurrentScene.GetChildren().OfType<Moneda>();
		return monedas.Count() > 0;
	}

	public void Spawn()
	{
		var moneda = MonedaScene.Instantiate<Moneda>();
		moneda.Recogida += OnMonedaRecogida;

		GetTree().CurrentScene.AddChild(moneda);

		float x = (float)GD.RandRange(GlobalPosition.X, GlobalPosition.X + Size.X);
		float y = (float)GD.RandRange(GlobalPosition.Y, GlobalPosition.Y + Size.Y);

		moneda.Position = new Vector2(x, y);
	}

	public void OnMonedaRecogida()
	{
		this.MonedasRecogidas += 1;

		// Emitimos la señal de que el jugador ha recogido una moneda.
		EmitSignal(SignalName.MonedaRecogida);
	}

	public void DestruirMonedas()
	{
		IEnumerable<Moneda> monedas = GetTree().CurrentScene.GetChildren().OfType<Moneda>();
		foreach (Moneda moneda in monedas)
			moneda.QueueFree();
	}
}
