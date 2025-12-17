using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.moneda;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class SpawnMonedas : Control
{

	public int MonedasRecogidas;

	// Señal "MonedaRecogida" para indicar que el jugador ha recogido una moneda.
	[Signal]
	public delegate void MonedaRecogidaEventHandler();

	[Export]
	public PackedScene MonedaScene;

	[Export]
	public int DistanciaMinima = 200;

	[Export]
	public Jugador Jugador { get; set; }

	public override void _Ready()
	{
		this.MonedasRecogidas = 0;
	}

	public override void _Process(double delta)
	{
		if (!ExisteMoneda())
			Spawn();
	}

	public bool ExisteMoneda()
	{
		var monedas = GetTree().CurrentScene.GetChildren().OfType<Moneda>();
		return monedas.Any();
	}

	public void Spawn()
	{
		LoggerJuego.Trace("Spawneamos una nueva moneda.");

		Vector2 centroJugador = Jugador?.GlobalPosition ?? Vector2.Inf;

		float x = (float)GD.RandRange(GlobalPosition.X, GlobalPosition.X + Size.X);
		float y = (float)GD.RandRange(GlobalPosition.Y, GlobalPosition.Y + Size.Y);

		Vector2 nuevaPosicionMoneda = new Vector2(x, y);

		if (Jugador != null)
		{
			while (UtilidadesMatematicas.PuntosCerca(centroJugador, nuevaPosicionMoneda, this.DistanciaMinima))
			{
				LoggerJuego.Info("La distancia de la nueva moneda está cerca del jugador. Generamos otro punto.");

				x = (float)GD.RandRange(GlobalPosition.X, GlobalPosition.X + Size.X);
				y = (float)GD.RandRange(GlobalPosition.Y, GlobalPosition.Y + Size.Y);

				nuevaPosicionMoneda = new Vector2(x, y);
			}
		}

		var moneda = MonedaScene.Instantiate<Moneda>();
		moneda.Recogida += OnMonedaRecogida;

		GetTree().CurrentScene.AddChild(moneda);

		moneda.Position = nuevaPosicionMoneda;
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