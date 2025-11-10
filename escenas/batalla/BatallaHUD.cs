namespace Primerjuego2D.escenas.batalla;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.ajustes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using static Primerjuego2D.nucleo.localizacion.GestorIdioma;

public partial class BatallaHUD : CanvasLayer
{
    public const long ID_OPCION_CASTELLANO = 0;
    public const long ID_OPCION_INGLES = 1;

    private Label _MessageLabel;
    private Label MessageLabel => _MessageLabel ??= GetNode<Label>("Message");

    private Timer _MessageTimer;
    private Timer MessageTimer => _MessageTimer ??= GetNode<Timer>("MessageTimer");

    private Button _StartButton;
    private Button StartButton => _StartButton ??= GetNode<Button>("StartButton");

    private Label _ScoreLabel;
    private Label ScoreLabel => _ScoreLabel ??= GetNode<Label>("ScoreLabel");

    private Label _MensajePausa;
    private Label MensajePausa => _MensajePausa ??= GetNode<Label>("MensajePausa");

    private MenuButton _MenuButtonLenguaje;
    private MenuButton MenuButtonLenguaje => _MenuButtonLenguaje ??= GetNode<MenuButton>("MenuButtonLenguaje");
    private Batalla _Batalla;
    private Batalla Batalla => _Batalla ??= GetParent<Batalla>();

    private BatallaControlador _Batallacontrolador;
    private BatallaControlador BatallaControlador => _Batallacontrolador ??= GetNode<BatallaControlador>("../BatallaControlador");


    public override void _Ready()
    {
        InicializarMenuButtonLenguaje();

        // Cambiamos el texto al inicial de la partida.
        this.MessageLabel.Text = "BatallaHUD.mensaje.esquivaLosEnemigos";
        this.MessageLabel.Show();
    }

    private void InicializarMenuButtonLenguaje()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguaje_IdPressed;

        Idioma idioma = GestorIdioma.GetIdiomaActual();
        switch (idioma)
        {
            default:
            case Idioma.Castellano:
                MenuButtonLenguaje_IdPressed(ID_OPCION_CASTELLANO);
                break;
            case Idioma.Ingles:
                MenuButtonLenguaje_IdPressed(ID_OPCION_INGLES);
                break;
        }
    }

    private void MenuButtonLenguaje_IdPressed(long id)
    {
        var popupMenu = MenuButtonLenguaje.GetPopup();

        // 🔹 Primero desmarcamos todos los ítems
        for (int i = 0; i < popupMenu.ItemCount; i++)
            popupMenu.SetItemChecked(i, false);

        // 🔹 Obtenemos el índice del ítem a partir de su ID
        int index = popupMenu.GetItemIndex((int)id);

        // 🔹 Marcamos solo el seleccionado
        popupMenu.SetItemChecked(index, true);

        switch (id)
        {
            default:
            case ID_OPCION_CASTELLANO:
                GestorIdioma.SetIdiomaCastellano();
                break;
            case ID_OPCION_INGLES:
                GestorIdioma.SetIdiomaIngles();
                break;
        }
    }

    async public void ShowStartMessage()
    {
        await UtilidadesNodos.EsperarRenaudar(this);
        this.MessageLabel.Text = "BatallaHUD.mensaje.preparate";
        this.MessageLabel.Show();

        await UtilidadesNodos.EsperarRenaudar(this);
        this.MessageTimer.Start();
    }

    async private void OnMessageTimerTimeout()
    {
        this.MessageLabel.Text = "BatallaHUD.mensaje.vamos";
        this.MessageLabel.Show();

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        this.MessageLabel.Hide();
    }

    async public void ShowGameOver()
    {
        await UtilidadesNodos.EsperarRenaudar(this);

        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        this.MessageLabel.Text = "BatallaHUD.mensaje.gameOver";
        this.MessageLabel.Show();

        // Esperamos 2 segundos.
        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        // Cambiamos el texto al inicial de la partida.
        this.MessageLabel.Text = "BatallaHUD.mensaje.esquivaLosEnemigos";
        this.MessageLabel.Show();

        // Mostramos lel botón de selección de idioma.
        this.MenuButtonLenguaje.Show();

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        // Mostramos el botón de start.
        this.StartButton.Show();
    }

    public void UpdateScore(int score)
    {
        this.ScoreLabel.Text = score.ToString();
    }

    private void OnStartButtonPressed()
    {
        this.StartButton.Hide();
        this.MenuButtonLenguaje.Hide();

        this.Batalla.NewGame();
    }

    Dictionary<CanvasItem, bool> visibilidadElementosPausa;

    public void OnPauseBattle()
    {
        if (Ajustes.JuegoPausado)
        {
            this.visibilidadElementosPausa = this.GetChildren()
                .OfType<CanvasItem>()
                .Where(item => item != this.MensajePausa && item != this.ScoreLabel)
                .ToDictionary(item => item, item => item.Visible);

            UtilidadesNodos.EsconderMenos(this, this.ScoreLabel);

            this.MensajePausa.Show();
        }
        else
        {
            var elementosVisibles = this.visibilidadElementosPausa
                .Where(kv => !kv.Key.Visible && kv.Value == true)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var elemento in elementosVisibles)
                elemento.Show();

            this.MensajePausa.Hide();
        }
    }
}
