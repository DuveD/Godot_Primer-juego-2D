namespace Primerjuego2D.escenas.batalla;

using System;
using System.Text.RegularExpressions;
using Godot;
using Primerjuego2D.escenas.entidades.enemigo;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades;


public partial class Batalla : Node
{
    [Export]
    public PackedScene EnemyScene { get; set; }

    private Timer _EnemyTimer;
    private Timer EnemyTimer => _EnemyTimer ??= GetNode<Timer>("EnemyTimer");

    private Timer _StartTimer;
    private Timer StartTimer => _StartTimer ??= GetNode<Timer>("StartTimer");

    private Timer _ScoreTimer;
    private Timer ScoreTimer => _ScoreTimer ??= GetNode<Timer>("ScoreTimer");

    private BatallaHUD _BatallaHUD;
    private BatallaHUD BatallaHUD => _BatallaHUD ??= GetNode<BatallaHUD>("BatallaHUD");

    private Jugador _Jugador;
    private Jugador Jugador => _Jugador ??= GetNode<Jugador>("Jugador");

    private Marker2D _StartPosition;
    private Marker2D StartPosition => _StartPosition ??= GetNode<Marker2D>("StartPosition");

    private PathFollow2D _MobSpawnLocation;
    private PathFollow2D MobSpawnLocation => _MobSpawnLocation ??= GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");


    private BatallaControlador _BatallaControlador;
    private BatallaControlador BatallaControlador => _BatallaControlador ??= GetNode<BatallaControlador>("BatallaControlador");

    private int Score;

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Pausable;
    }

    public void NewGame()
    {
        Enemigo.DeleteAllEnemies(this);

        this.Score = 0;

        this.Jugador.Start(this.StartPosition.Position);
        this.StartTimer.Start();

        this.BatallaHUD.UpdateScore(Score);
        this.BatallaHUD.ShowStartMessage();

        this.BatallaControlador.IniciarBatalla();
    }
    public void JugadorGolpeadoPorEnemigo()
    {
        GameOver();
    }

    public void GameOver()
    {
        this.EnemyTimer.Stop();
        this.ScoreTimer.Stop();

        this.BatallaControlador.FinalizarBatalla();
    }

    private void OnScoreTimerTimeout()
    {
        this.Score++;
        this.BatallaHUD.UpdateScore(Score);
    }

    private void OnStartTimerTimeout()
    {
        this.EnemyTimer.Start();
        this.ScoreTimer.Start();
    }

    private void OnEnemyTimerTimeout()
    {
        // Creamos una nueva instancia de un enemigo.
        Enemigo enemigo = EnemyScene.Instantiate<Enemigo>();

        // Elegimos una localización aleatória del path 2D de los enemigos.
        this.MobSpawnLocation.ProgressRatio = Randomizer.GetRandomFloat();

        // Set the mob's position to a random location.
        enemigo.Position = this.MobSpawnLocation.Position;

        // Informamos la dirección del sprite enemigo. Perpendicular a la dirección del path 2D de los enemigos. 
        float direction = this.MobSpawnLocation.RotationDegrees + 90;

        // Randomizamos la dirección, de -45 a 45.
        direction += (float)Randomizer.GetRandomInt(-45, 45);
        enemigo.RotationDegrees = direction;

        // Informamos el vector de velocidad y dirección.
        Vector2 velocity = new Vector2((float)Randomizer.GetRandomDouble(150.0, 250.0), 0);
        float directionRad = (float)UtilidadesMatematicas.DegreesToRadians(direction);
        enemigo.LinearVelocity = velocity.Rotated(directionRad);

        // Spawneamos el enemigo en la escena principal.
        this.AddChild(enemigo);
    }
}
