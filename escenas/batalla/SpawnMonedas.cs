using System;
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
	public delegate void MonedaRecogidaEventHandler(Moneda moneda);

	[Export]
	public PackedScene MonedaScene;

	[Export]
	public PackedScene MonedaEspecialScene;

	[Export]
	public int DistanciaMinima = 200;

	[Export]
	public Jugador Jugador { get; set; }

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		this.MonedasRecogidas = 0;

		Spawn();
	}

	public void Spawn()
	{
		LoggerJuego.Trace("Spawneamos una nueva moneda.");

		// Moneda normal
		SpawnMoneda();

		bool existeMonedaEspecial = UtilidadesNodos.ObtenerNodosDeTipo<MonedaEspecial>(GetTree().CurrentScene).Any();
		if (!existeMonedaEspecial)
		{
			// Moneda especial según probabilidad
			float prob = 0.1f + 0.4f * Mathf.Exp(-0.046f * MonedasRecogidas);
			if (Randomizador.GetRandomFloat() < prob)
			{
				LoggerJuego.Trace("Spawneamos una moneda especial.");
				SpawnMoneda(true);
			}
		}
	}

	private Moneda SpawnMoneda(bool monedaEspecial = false)
	{
		Moneda moneda;

		if (monedaEspecial)
		{
			moneda = MonedaEspecialScene.Instantiate<MonedaEspecial>();
		}
		else
		{
			moneda = MonedaScene.Instantiate<Moneda>();
		}

		moneda.Recogida += OnMonedaRecogida;

		GetTree().CurrentScene.AddChild(moneda);
		moneda.Position = ObtenerPosicionAleatoriaSegura();

		return moneda;
	}

	private Vector2 ObtenerPosicionAleatoriaSegura()
	{
		Vector2 nuevaPos;
		Vector2 centroJugador = Jugador.GlobalPosition;
		bool cercaJugador;

		List<Moneda> monedas = UtilidadesNodos.ObtenerNodosDeTipo<Moneda>(GetTree().CurrentScene);
		bool cercaOtraMoneda;

		do
		{
			float x = (float)Randomizador.GetRandomDouble(GlobalPosition.X, GlobalPosition.X + Size.X);
			float y = (float)Randomizador.GetRandomDouble(GlobalPosition.Y, GlobalPosition.Y + Size.Y);
			nuevaPos = new Vector2(x, y);

			cercaJugador = UtilidadesMatematicas.PuntosCerca(centroJugador, nuevaPos, this.DistanciaMinima);
			cercaOtraMoneda = monedas.Any(moneda => UtilidadesMatematicas.PuntosCerca(moneda.Position, nuevaPos, moneda.ObtenerRadioCollisionShape2D() * 2));
		}
		while (cercaJugador || cercaOtraMoneda);

		return nuevaPos;
	}

	public void OnMonedaRecogida(Moneda moneda)
	{
		this.MonedasRecogidas += 1;

		// Emitimos la señal de que el jugador ha recogido una moneda.
		EmitSignal(SignalName.MonedaRecogida, moneda);

		bool esMonedaEspecial = moneda is MonedaEspecial;
		if (!esMonedaEspecial)
			CallDeferred(nameof(Spawn));
	}

	public void DestruirMonedas()
	{
		IEnumerable<Moneda> monedas = GetTree().CurrentScene.GetChildren().OfType<Moneda>();
		foreach (Moneda moneda in monedas)
			moneda.QueueFree();
	}
}