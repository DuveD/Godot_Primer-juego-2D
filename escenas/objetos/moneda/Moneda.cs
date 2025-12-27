using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.modelos.interfaces;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.objetos.moneda;

[AtributoNivelLog(NivelLog.Info)]
public partial class Moneda : Consumible
{
	[Export]
	public int Valor { get; set; } = 1;

	[Export]
	public float VelocidadAnimacion { get; set; } = 1.0f;

	// Si es -1, no se autodestruye. Si >0, se destruye automáticamente después de ese tiempo.
	[Export]
	public float TiempoDestruccion { get; set; } = -1f;

	[Export]
	public Color ColorTextoFlotante { get; set; } = Colors.Gold;

	[Signal]
	public delegate void RecogidaEventHandler(Moneda moneda);

	private CollisionShape2D _CollisionShape2D;
	public CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

	public static readonly PackedScene TextoFlotanteScene = GD.Load<PackedScene>(UtilidadesNodos.ObtenerRutaEscena<TextoFlotante>());

	private AnimationPlayer _AnimationPlayerRotacion;
	public AnimationPlayer AnimationPlayerRotacion => _AnimationPlayerRotacion ??= GetNode<AnimationPlayer>("AnimationPlayerRotacion");

	private Timer _TimerDestruccion;

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.OBJETOS);

		this.AnimationPlayerRotacion.SpeedScale = this.VelocidadAnimacion;

		// Configuramos timer de autodestrucción
		if (TiempoDestruccion > 0)
		{
			_TimerDestruccion = new Timer();
			_TimerDestruccion.WaitTime = TiempoDestruccion;
			_TimerDestruccion.OneShot = true;
			_TimerDestruccion.Autostart = true;
			_TimerDestruccion.Timeout += OnTimerDestruccionTimeout;
			AddChild(_TimerDestruccion);
		}
	}

	public override void OnRecogida(Jugador jugador)
	{
		LoggerJuego.Info("Moneda (" + this.Valor + ") recogida.");

		EmitSignal(SignalName.Recogida, this);

		Global.GestorAudio.ReproducirSonido("retro_coin.mp3");

		MostrarTextoFlotante();

		// Cancelamos el timer si estaba activo.
		_TimerDestruccion?.Stop();

		// Usamos CallDeferred para evitar conflictos si el spawn ocurre durante la señal.
		CallDeferred(Node.MethodName.QueueFree);
	}

	private void OnTimerDestruccionTimeout()
	{
		LoggerJuego.Trace("Moneda autodestruida tras " + TiempoDestruccion + " segundos.");

		// Usamos CallDeferred para que no choque con signals o procesamiento actual.
		CallDeferred(Node.MethodName.QueueFree);
	}

	public virtual void MostrarTextoFlotante()
	{
		var texto = TextoFlotanteScene.Instantiate<TextoFlotante>();

		texto.Texto = " +" + this.Valor.ToString();
		texto.Color = ColorTextoFlotante;
		texto.PosicionGlobal = GlobalPosition;

		GetTree().CurrentScene.AddChild(texto);
	}

	public float obtenerRadioCollisionShape2D()
	{
		return ((CircleShape2D)CollisionShape2D?.Shape).Radius;
	}
}
