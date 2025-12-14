
using Godot;
using Primerjuego2D.escenas;

namespace Primerjuego2D.nucleo.modelos;

public partial class BotonMenuPrincipal : Button
{
    private bool _reproducirSonido = true;

    public override void _Ready()
    {
        this.FocusEntered += OnFocusedEntered;
        this.MouseEntered += OnMouseEntered;
    }

    public void OnFocusedEntered()
    {
        if (this._reproducirSonido)
            Global.GestorAudio.ReproducirSonido("kick.mp3");
    }

    public void OnMouseEntered()
    {
        Global.GestorAudio.ReproducirSonido("kick.mp3");
    }

    public void GrabFocusSilencioso()
    {
        this._reproducirSonido = false;
        this.GrabFocus();
        this._reproducirSonido = true;
    }
}