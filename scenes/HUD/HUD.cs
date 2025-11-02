using Godot;
using System;

public partial class HUD : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

    private Label MessageLabel => GetNode<Label>("Message");

    private Timer MessageTimer => GetNode<Timer>("MessageTimer");

    private Button StartButton => GetNode<Button>("StartButton");

    private Label ScoreLabel => GetNode<Label>("ScoreLabel");

    public override void _Ready()
    {
        // Cambiamos el texto al inicial de la partida.
        this.MessageLabel.Text = "Dodge the\nCreeps!";
        this.MessageLabel.Show();
    }

    internal void ShowStartMessage()
    {
        this.MessageLabel.Text = "Get Ready!";
        this.MessageLabel.Show();

        this.MessageTimer.Start();
    }

    async private void OnMessageTimerTimeout()
    {
        this.MessageLabel.Text = "GO!";
        this.MessageLabel.Show();

        // Creamos un timer de 1 segundo y esperamos.
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        this.MessageLabel.Hide();
    }

    async public void ShowGameOver()
    {
        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        this.MessageLabel.Text = "Game Over";
        this.MessageLabel.Show();

        // Esperamos 1 segundo.
        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

        // Cambiamos el texto al inicial de la partida.
        this.MessageLabel.Text = "Dodge the\nCreeps!";
        this.MessageLabel.Show();

        // Creamos un timer de 1 segundo y esperamos.
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        this.StartButton.Show();
    }

    public void UpdateScore(int score)
    {
        this.ScoreLabel.Text = score.ToString();
    }

    private void OnStartButtonPressed()
    {
        this.StartButton.Hide();
        EmitSignal(SignalName.StartGame);
    }
}
