using Godot;


namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ButtonSalir : Button
{
	public override void _Ready()
	{
		this.FocusEntered += OnFocusedEntered;
		this.MouseEntered += OnMouseEntered;
	}

	public void OnFocusedEntered()
	{
		Global.GestorAudio.ReproducirSonido("kick.mp3");
	}

	public void OnMouseEntered()
	{
		Global.GestorAudio.ReproducirSonido("kick.mp3");
	}
}