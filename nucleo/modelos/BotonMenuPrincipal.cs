
using Godot;
using Primerjuego2D.escenas;

namespace Primerjuego2D.nucleo.modelos;

public partial class BotonMenuPrincipal : Button
{
    public bool ReproducirSonido { get; set; } = true;

    public override void _Ready()
    {
        this.FocusEntered += OnFocusedEntered;
        this.MouseEntered += OnMouseEntered;
    }

    public void OnFocusedEntered()
    {
        if (this.ReproducirSonido)
            Global.GestorAudio.ReproducirSonido("kick.mp3");
    }

    public void OnMouseEntered()
    {
        Global.GestorAudio.ReproducirSonido("kick.mp3");
    }

    public void GrabFocusSilencioso()
    {
        this.ReproducirSonido = false;
        this.GrabFocus();
        this.ReproducirSonido = true;
    }
}