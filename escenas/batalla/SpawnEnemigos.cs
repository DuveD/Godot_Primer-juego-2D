using System;
using System.Diagnostics;
using Godot;
using Primerjuego2D.escenas.entidades.enemigo;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

public partial class SpawnEnemigos : Control
{
    [Signal]
    public delegate void EnemigoSpawneadoEventHandler(Enemigo enemigo);

    private Timer _TimerSpawnEnemigo;
    public Timer TimerSpawnEnemigo => _TimerSpawnEnemigo ??= GetNode<Timer>("TimerSpawnEnemigo");

    private PathFollow2D _EnemySpawnLocation;
    public PathFollow2D EnemySpawnLocation => _EnemySpawnLocation ??= GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");

    [Export]
    public PackedScene PackedSceneEnemigo { get; set; }

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.TimerSpawnEnemigo.Timeout += SpawnEnemigo;
    }

    public void Start()
    {
        this.TimerSpawnEnemigo.Start();
        SpawnEnemigo();
    }

    public void Stop()
    {
        this.TimerSpawnEnemigo.Stop();
    }

    private void SpawnEnemigo()
    {
        // Creamos una nueva instancia de un enemigo.
        Enemigo enemigo = PackedSceneEnemigo.Instantiate<Enemigo>();

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

        EmitSignal(SignalName.EnemigoSpawneado, enemigo);
    }
}
