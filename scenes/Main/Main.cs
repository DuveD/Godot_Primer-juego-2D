using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Main : Node
{
    [Export]
    public PackedScene EnemyScene { get; set; }

    private Timer EnemyTimer => GetNode<Timer>("EnemyTimer");

    private Timer StartTimer => GetNode<Timer>("StartTimer");

    private Timer ScoreTimer => GetNode<Timer>("ScoreTimer");

    private HUD HUD => GetNode<HUD>("HUD");

    private Player Player => GetNode<Player>("Player");

    private Marker2D StartPosition => GetNode<Marker2D>("StartPosition");

    private int _score;

    public override void _Ready()
    {
    }

    public void NewGame()
    {
        Enemy.DeleteAll(this);

        this._score = 0;

        this.Player.Start(this.StartPosition.Position);
        this.StartTimer.Start();

        this.HUD.UpdateScore(_score);
        this.HUD.ShowStartMessage();
    }

    public void GameOver()
    {
        this.EnemyTimer.Stop();
        this.ScoreTimer.Stop();

        this.HUD.ShowGameOver();
    }

    private void OnScoreTimerTimeout()
    {
        this._score++;
        this.HUD.UpdateScore(_score);
    }

    private void OnStartTimerTimeout()
    {
        this.EnemyTimer.Start();
        this.ScoreTimer.Start();
    }

    private void OnEnemyTimerTimeout()
    {
        // Creamos una nueva instancia de un enemigo.
        Enemy enemy = EnemyScene.Instantiate<Enemy>();

        // Elegimos una localización aleatória del path 2D de los enemigos.
        PathFollow2D mobSpawnLocation = GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");
        mobSpawnLocation.ProgressRatio = Randomizer.GetRandomFloat();

        // Set the mob's position to a random location.
        enemy.Position = mobSpawnLocation.Position;

        // Informamos la dirección del sprite enemigo. Perpendicular a la dirección del path 2D de los enemigos. 
        float direction = mobSpawnLocation.RotationDegrees + 90;

        // Randomizamos la dirección, de -45 a 45.
        direction += (float)Randomizer.GetRandomInt(-45, 45);
        enemy.RotationDegrees = direction;

        // Informamos el vector de velocidad y dirección.
        Vector2 velocity = new Vector2((float)Randomizer.GetRandomDouble(150.0, 250.0), 0);
        float directionRad = (float)MathUtilities.DegreesToRadians(direction);
        enemy.LinearVelocity = velocity.Rotated(directionRad);

        // Spawneamos el enemigo en la escena principal.
        this.AddChild(enemy);
    }
}
