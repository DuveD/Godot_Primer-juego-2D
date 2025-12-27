using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class BatallaHUD : CanvasLayer
{
    private Label _MessageLabel;
    private Label MessageLabel => _MessageLabel ??= GetNode<Label>("Message");

    private Label _ScoreLabel;
    private Label ScoreLabel => _ScoreLabel ??= GetNode<Label>("ScoreLabel");

    private Label _MensajePausa;
    private Label MensajePausa => _MensajePausa ??= GetNode<Label>("MensajePausa");

    Dictionary<CanvasItem, bool> VisibilidadElementosPausa;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");
    }

    async public void MostrarMensajesIniciarBatalla()
    {
        // Cambiamos el texto al inicial de la partida.
        ActualizarMensaje("BatallaHUD.mensaje.preparate");

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        ActualizarMensaje("BatallaHUD.mensaje.vamos");

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        MostrarMensaje(false);
    }

    async private void MostrarMensajeGameOver()
    {
        await UtilidadesNodos.EsperarRenaudar(this);

        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        ActualizarMensaje("BatallaHUD.mensaje.gameOver");
        this.MessageLabel.Show();
    }

    public void ActualizarMensaje(string mensaje)
    {
        this.MessageLabel.Text = mensaje;
    }

    public void MostrarMensaje(bool mostrar)
    {
        if (mostrar)
            this.MessageLabel.Show();
        else
            this.MessageLabel.Hide();
    }

    public void ActualizarPuntuacion(int score)
    {
        this.ScoreLabel.Text = score.ToString();
    }

    private void EsconderHUD()
    {
        this.VisibilidadElementosPausa = this.GetChildren()
            .OfType<CanvasItem>()
            .Where(item => item != this.MensajePausa && item != this.ScoreLabel)
            .ToDictionary(item => item, item => item.Visible);

        UtilidadesNodos.EsconderMenos(this, this.ScoreLabel);

        MostrarMensajePausa(true);
    }

    private void MostrarHUD()
    {
        MostrarMensajePausa(false);

        var elementosVisibles = this.VisibilidadElementosPausa
            .Where(kv => !kv.Key.Visible && kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var elemento in elementosVisibles)
            elemento.Show();
    }

    public void MostrarMensajePausa(bool mostrar)
    {
        if (mostrar)
            this.MensajePausa.Hide();
        else
            this.MensajePausa.Show();
    }
}
