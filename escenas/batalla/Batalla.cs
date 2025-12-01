using Godot;
using Primerjuego2D.escenas.entidades.enemigo;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class Batalla : Node
{
    [Signal]
    public delegate void GameOverFinalizadoEventHandler();

    [Export]
    public PackedScene EnemyScene { get; set; }

    ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private Timer _EnemyTimer;
    private Timer EnemyTimer => _EnemyTimer ??= GetNode<Timer>("EnemyTimer");

    private Timer _StartTimer;
    private Timer StartTimer => _StartTimer ??= GetNode<Timer>("StartTimer");

    private BatallaHUD _BatallaHUD;
    private BatallaHUD BatallaHUD => _BatallaHUD ??= GetNode<BatallaHUD>("BatallaHUD");

    private Jugador _Jugador;
    private Jugador Jugador => _Jugador ??= GetNode<Jugador>("Jugador");

    private Marker2D _StartPosition;
    private Marker2D StartPosition => _StartPosition ??= GetNode<Marker2D>("StartPosition");

    private PathFollow2D _EnemySpawnLocation;
    private PathFollow2D EnemySpawnLocation => _EnemySpawnLocation ??= GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");

    private BatallaControlador _BatallaControlador;
    private BatallaControlador BatallaControlador => _BatallaControlador ??= GetNode<BatallaControlador>("BatallaControlador");

    private int Score;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        ProcessMode = ProcessModeEnum.Pausable;

        this.NuevoJuego();
    }

    public void NuevoJuego()
    {
        Enemigo.DeleteAllEnemies(this);

        this.Score = 0;

        this.Jugador.Start(this.StartPosition.Position);
        this.StartTimer.Start();

        this.BatallaHUD.ActualizarPuntuacion(Score);
        this.BatallaHUD.MostrarMensajePreparate();

        this.BatallaControlador.IniciarBatalla();
    }

    public async void GameOver()
    {
        this.EnemyTimer.Stop();

        this.BatallaControlador.FinalizarBatalla();

        await UtilidadesNodos.EsperarSegundos(this, 2.0);

        EmitSignal(SignalName.GameOverFinalizado);
    }

    public void SumarPuntuacion()
    {
        this.Score++;
        this.BatallaHUD.ActualizarPuntuacion(Score);
    }

    private void OnStartTimerTimeout()
    {
        this.EnemyTimer.Start();
    }

    private void OnEnemyTimerTimeout()
    {
        // Creamos una nueva instancia de un enemigo.
        Enemigo enemigo = EnemyScene.Instantiate<Enemigo>();

        // Elegimos una localización aleatória del path 2D de los enemigos.
        this.EnemySpawnLocation.ProgressRatio = Randomizador.GetRandomFloat();

        // Set the mob's position to a random location.
        enemigo.Position = this.EnemySpawnLocation.Position;

        // Informamos la dirección del sprite enemigo. Perpendicular a la dirección del path 2D de los enemigos. 
        float direction = this.EnemySpawnLocation.RotationDegrees + 90;

        // Randomizamos la dirección, de -45 a 45.
        direction += (float)Randomizador.GetRandomInt(-45, 45);
        enemigo.RotationDegrees = direction;

        // Informamos el vector de velocidad y dirección.
        Vector2 velocity = new Vector2((float)Randomizador.GetRandomDouble(150.0, 250.0), 0);
        float directionRad = (float)UtilidadesMatematicas.DegreesToRadians(direction);
        enemigo.LinearVelocity = velocity.Rotated(directionRad);

        // Spawneamos el enemigo en la escena principal.
        this.AddChild(enemigo);
    }
}
